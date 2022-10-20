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


        public RepairController(
            ISaveRepairRequestUseCase saveRepairRequestUseCase,
            IInternalEmailSender internalEmailSender,
            IAppointmentConfirmationSender appointmentConfirmationSender,
            IBookAppointmentUseCase bookAppointmentUseCase,
            IRetrieveRepairsUseCase retrieveRepairsUseCase
        )
        {
            this.saveRepairRequestUseCase = saveRepairRequestUseCase;
            this.internalEmailSender = internalEmailSender;
            this.appointmentConfirmationSender = appointmentConfirmationSender;
            this.bookAppointmentUseCase = bookAppointmentUseCase;
            this.retrieveRepairsUseCase = retrieveRepairsUseCase;
        }

        // [HttpPost]
        // public async Task<IActionResult> SaveRepair([FromBody] RepairRequest repairRequest)
        // {
        //     try
        //     {
        //         var result = await saveRepairRequestUseCase.Execute(repairRequest);
        //         await bookAppointmentUseCase.Execute(result.Id, result.SOR, result.Address.LocationId,
        //             result.Time.StartDateTime, result.Time.EndDateTime, result.Description.Text);
        //         appointmentConfirmationSender.Execute(result);
        //         await internalEmailSender.Execute(result);
        //         return Ok(result.Id);
        //     }
        //     catch (Exception ex)
        //     {
        //         SentrySdk.CaptureException(ex);
        //         return StatusCode(500, ex.Message);
        //     }
        // }

        [HttpGet]
        [Route("PropertyRepairsCommunal")]
        public async Task<IActionResult> PropertyRepairsCommunal([FromQuery] string propertyReference)
        {
            try
            {
                var result = await retrieveRepairsUseCase.Execute(RepairTypeHelper.CommunalRepairType, propertyReference);
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
            try
            {
                return await SaveRepair(repairRequest, RepairTypeHelper.TenantRepairType);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("CommunalRepair")]
        public async Task<IActionResult> CommunalRepair([FromBody] RepairRequest repairRequest)
        {
            try
            {
                return await SaveRepair(repairRequest, RepairTypeHelper.CommunalRepairType);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<IActionResult> SaveRepair(RepairRequest repairRequest, string repairType)
        {
            var result = await saveRepairRequestUseCase.Execute(repairRequest, RepairTypeHelper.TenantRepairType);
            await bookAppointmentUseCase.Execute(result.Id, result.SOR, result.Address.LocationId,
                result.Time.StartDateTime, result.Time.EndDateTime, result.Description.Text);
            appointmentConfirmationSender.Execute(result);
            await internalEmailSender.Execute(result);
            return Ok(result.Id);
        }
    }
}
