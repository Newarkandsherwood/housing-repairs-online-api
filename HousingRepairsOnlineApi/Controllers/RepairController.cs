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
        private readonly IRepairDurationHelper repairDurationHelper;

        public RepairController(
            ISaveRepairRequestUseCase saveRepairRequestUseCase,
            IInternalEmailSender internalEmailSender,
            IAppointmentConfirmationSender appointmentConfirmationSender,
            IBookAppointmentUseCase bookAppointmentUseCase,
            IRetrieveRepairsUseCase retrieveRepairsUseCase,
            IRetrieveAvailableCommunalAppointmentUseCase retrieveAvailableCommunalAppointmentUseCase,
            IRepairDurationHelper repairDurationHelper)
        {
            this.saveRepairRequestUseCase = saveRepairRequestUseCase;
            this.internalEmailSender = internalEmailSender;
            this.appointmentConfirmationSender = appointmentConfirmationSender;
            this.bookAppointmentUseCase = bookAppointmentUseCase;
            this.retrieveRepairsUseCase = retrieveRepairsUseCase;
            this.repairDurationHelper = repairDurationHelper;
            this.retrieveAvailableCommunalAppointmentUseCase = retrieveAvailableCommunalAppointmentUseCase;
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

        [HttpPost]
        [Route("TenantRepair")]
        public async Task<IActionResult> TenantRepair([FromBody] RepairRequest repairRequest)
        {
            return await SaveRepair(RepairType.Tenant, repairRequest);
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
                repairRequest.Time = new RepairAvailability
                {
                    StartDateTime = appointment.StartTime,
                    EndDateTime = appointment.EndTime,
                };
            }

            return appointmentFound;
        }

        internal async Task<IActionResult> SaveRepair(string repairType, RepairRequest repairRequest)
        {
            try
            {
                var result = await saveRepairRequestUseCase.Execute(repairType, repairRequest);

                await bookAppointmentUseCase.Execute(result.Id, result.SOR, result.Priority, result.Address.LocationId,
                    result.Time.StartDateTime, result.Time.EndDateTime, result.Description.Text);

                appointmentConfirmationSender.Execute(result);
                await internalEmailSender.Execute(result);
                var daysForRepair = repairDurationHelper.GetDaysForRepair(result);
                var repairBookingResult = new RepairBookingResult() { Id = result.Id, DaysForRepair = daysForRepair };
                return Ok(repairBookingResult);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
