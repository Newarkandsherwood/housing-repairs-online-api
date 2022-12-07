using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepairController : ControllerBase
    {
        private readonly ISaveRepairRequestUseCase saveRepairRequestUseCase;
        private readonly IAppointmentConfirmationSender appointmentConfirmationSender;
        private readonly IBookAppointmentUseCase bookAppointmentUseCase;
        private readonly IInternalEmailSender internalEmailSender;
        private readonly IRetrieveRepairsUseCase retrieveRepairsUseCase;
        private readonly IRetrieveAvailableCommunalAppointmentUseCase retrieveAvailableCommunalAppointmentUseCase;
        private readonly IAppointmentTimeToRepairAvailabilityMapper appointmentTimeToRepairAvailabilityMapper;
        private readonly IRepairToFindRepairResponseMapper repairToFindRepairResponseMapper;
        private readonly ICancelAppointmentUseCase cancelAppointmentUseCase;
        private readonly ICancelRepairRequestUseCase cancelRepairRequestUseCase;

        public RepairController(
            ISaveRepairRequestUseCase saveRepairRequestUseCase,
            IInternalEmailSender internalEmailSender,
            IAppointmentConfirmationSender appointmentConfirmationSender,
            IBookAppointmentUseCase bookAppointmentUseCase,
            IRetrieveRepairsUseCase retrieveRepairsUseCase,
            IRetrieveAvailableCommunalAppointmentUseCase retrieveAvailableCommunalAppointmentUseCase,
            IAppointmentTimeToRepairAvailabilityMapper appointmentTimeToRepairAvailabilityMapper,
            IRepairToFindRepairResponseMapper repairToFindRepairResponseMapper,
            ICancelAppointmentUseCase cancelAppointmentUseCase,
            ICancelRepairRequestUseCase cancelRepairRequestUseCase)
        {
            this.saveRepairRequestUseCase = saveRepairRequestUseCase;
            this.internalEmailSender = internalEmailSender;
            this.appointmentConfirmationSender = appointmentConfirmationSender;
            this.bookAppointmentUseCase = bookAppointmentUseCase;
            this.retrieveRepairsUseCase = retrieveRepairsUseCase;
            this.retrieveAvailableCommunalAppointmentUseCase = retrieveAvailableCommunalAppointmentUseCase;
            this.appointmentTimeToRepairAvailabilityMapper = appointmentTimeToRepairAvailabilityMapper;
            this.repairToFindRepairResponseMapper = repairToFindRepairResponseMapper;
            this.cancelAppointmentUseCase = cancelAppointmentUseCase;
            this.cancelRepairRequestUseCase = cancelRepairRequestUseCase;
        }

        [HttpGet]
        [Route("CommunalPropertyRepairs")]
        public async Task<IActionResult> CommunalPropertyRepairs([FromQuery] string propertyReference)
        {
            try
            {
                var result = await retrieveRepairsUseCase.Execute(RepairType.Communal, propertyReference);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("TenantOrLeaseholdPropertyRepair")]
        public async Task<IActionResult> TenantOrLeaseholdPropertyRepair([FromQuery] string postcode, [FromQuery] string repairId)
        {
            try
            {
                var result = await retrieveRepairsUseCase.Execute(
                    new[] { RepairType.Tenant, RepairType.Leasehold },
                    postcode, repairId);

                if (result == null)
                {
                    return NotFound("Repair request not found for postcode and repairId provided.");
                }

                var response = repairToFindRepairResponseMapper.Map(result);

                return Ok(response);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("TenantOrLeaseholdPropertyRepairCancel")]
        public async Task<IActionResult> TenantOrLeaseholdPropertyRepairCancel([FromQuery] string postcode, [FromQuery] string repairId)
        {
            try
            {
                var repair = await retrieveRepairsUseCase.Execute(
                    new[] { RepairType.Tenant, RepairType.Leasehold },
                    postcode, repairId);

                if (repair == null)
                {
                    return NotFound("Repair request not found for postcode and repairId provided");
                }

                if (repair.Status == RepairStatus.Cancelled)
                {
                    return Ok("The repair has already been cancelled in Housing Repairs Online");
                }

                try
                {
                    var cancelAppointmentStatus = await cancelAppointmentUseCase.Execute(repairId);
                    switch (cancelAppointmentStatus)
                    {
                        case CancelAppointmentStatus.Found:
                            await cancelRepairRequestUseCase.Execute(repair);
                            break;
                        case CancelAppointmentStatus.Error:
                        case CancelAppointmentStatus.NotFound:
                             return StatusCode(500, "Error updating the appointment");
                    }
                    return Ok("The repair has successfully been cancelled");
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("TenantRepair")]
        public async Task<IActionResult> TenantRepair([FromBody] RepairRequest repairRequest)
        {
            return await SaveRepair(RepairType.Tenant, repairRequest);
        }

        [HttpPost]
        [Route("LeaseholdRepair")]
        public async Task<IActionResult> LeaseholdRepair([FromBody] RepairRequest repairRequest)
        {
            return await SaveRepair(RepairType.Leasehold, repairRequest);
        }

        [HttpPost]
        [Route("CommunalRepair")]
        public async Task<IActionResult> CommunalRepair([FromBody] RepairRequest repairRequest)
        {
            var updateSuccessful = await UpdateRepairRequestWithCommunalAppointment(repairRequest);
            if (!updateSuccessful)
            {
                var statusMessage = "No available appointment found";
                SentrySdk.CaptureMessage(statusMessage);
                return StatusCode(500, statusMessage);
            }

            return await SaveRepair(RepairType.Communal, repairRequest);
        }

        private async Task<bool> UpdateRepairRequestWithCommunalAppointment(RepairRequest repairRequest)
        {
            var appointment = await retrieveAvailableCommunalAppointmentUseCase.Execute(
                repairRequest.Location.Value,
                repairRequest.Problem.Value, repairRequest.Issue.Value, repairRequest.Address.LocationId);

            var appointmentFound = appointment != null;
            if (appointmentFound)
            {
                repairRequest.Time = appointmentTimeToRepairAvailabilityMapper.Map(appointment);
            }

            return appointmentFound;
        }

        internal async Task<IActionResult> SaveRepair(string repairType, RepairRequest repairRequest)
        {
            try
            {
                var result = await saveRepairRequestUseCase.Execute(repairType, repairRequest);

                // await bookAppointmentUseCase.Execute(result.Id, result.SOR, result.Priority, result.Address.LocationId,
                //     result.Time.StartDateTime, result.Time.EndDateTime, result.Description.Text);
                //
                // appointmentConfirmationSender.Execute(result);
                // await internalEmailSender.Execute(result);
                return Ok(result.Id);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
