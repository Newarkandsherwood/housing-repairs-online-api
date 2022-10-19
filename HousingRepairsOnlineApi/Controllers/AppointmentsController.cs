using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase;

        public AppointmentsController(IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase)
        {
            this.retrieveAvailableAppointmentsUseCase = retrieveAvailableAppointmentsUseCase;
        }

        [HttpGet]
        [Route("AvailableAppointments")]
        public async Task<IActionResult> AvailableAppointments(
            [FromQuery] string repairLocation,
            [FromQuery] string repairProblem,
            [FromQuery] string repairIssue,
            [FromQuery] string locationId,
            [FromQuery] DateTime? fromDate = null)
        {
            try
            {
                var result = await retrieveAvailableAppointmentsUseCase.Execute(RepairType.Tenant, repairLocation, repairProblem, repairIssue, locationId, fromDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
