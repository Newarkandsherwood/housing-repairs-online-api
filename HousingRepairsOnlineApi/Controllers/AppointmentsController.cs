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
        [Route("AvailableTenantAppointments")]
        public async Task<IActionResult> AvailableTenantAppointments(
            [FromQuery] string repairLocation,
            [FromQuery] string repairProblem,
            [FromQuery] string repairIssue,
            [FromQuery] string locationId,
            [FromQuery] DateTime? fromDate = null)
        {
            return await AvailableAppointments(RepairType.Tenant, repairLocation, repairProblem, repairIssue, locationId, fromDate);
        }

        internal async Task<IActionResult> AvailableAppointments(string repairType,
            string repairLocation,
            string repairProblem,
            string repairIssue,
            string locationId,
            DateTime? fromDate = null)
        {
            try
            {
                var result = await retrieveAvailableAppointmentsUseCase.Execute(repairType, repairLocation, repairProblem, repairIssue, locationId, fromDate);
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
