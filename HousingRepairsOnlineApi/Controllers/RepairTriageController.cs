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
        public async Task<IActionResult> JourneyRepairTriageOptions(string emergencyValue, string notEligibleNonEmergencyValue, string unableToBookValue)
        {
            if (string.IsNullOrWhiteSpace(emergencyValue))
            {
                return StatusCode(400, $"{nameof(emergencyValue)} is mandatory and must be non-empty and not whitespace");
            }
            if (string.IsNullOrWhiteSpace(notEligibleNonEmergencyValue))
            {
                return StatusCode(400, $"{nameof(notEligibleNonEmergencyValue)} is mandatory and must be non-empty and not whitespace");
            }
            if (string.IsNullOrWhiteSpace(unableToBookValue))
            {
                return StatusCode(400, $"{nameof(unableToBookValue)} is mandatory and must be non-empty and not whitespace");
            }

            try
            {
                var result = await retrieveJourneyTriageOptionsUseCase.Execute(emergencyValue, notEligibleNonEmergencyValue, unableToBookValue);
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
