using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepairTriageController : ControllerBase
    {
        private readonly IRetrieveJourneyTriageOptionsUseCase retrieveJourneyTriageOptionsUseCase;

        public RepairTriageController(IRetrieveJourneyTriageOptionsUseCase retrieveJourneyTriageOptionsUseCase)
        {
            this.retrieveJourneyTriageOptionsUseCase = retrieveJourneyTriageOptionsUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> JourneyRepairTriageOptions()
        {
            try
            {
                var result = await retrieveJourneyTriageOptionsUseCase.Execute();
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
