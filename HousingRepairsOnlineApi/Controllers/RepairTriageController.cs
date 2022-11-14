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
    public class RepairTriageController : ControllerBase
    {
        private readonly IRetrieveJourneyTriageOptionsUseCase retrieveJourneyTriageOptionsUseCase;

        public RepairTriageController(IRetrieveJourneyTriageOptionsUseCase retrieveJourneyTriageOptionsUseCase)
        {
            this.retrieveJourneyTriageOptionsUseCase = retrieveJourneyTriageOptionsUseCase;
        }

        [HttpGet]
        [Route("TenantRepairTriageOptions")]
        public async Task<IActionResult> TenantRepairTriageOptions(string emergencyValue,
            string notEligibleNonEmergencyValue, string unableToBookValue, string contactUsValue)
        {
            return await JourneyRepairTriageOptions(RepairType.Tenant, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue, contactUsValue);
        }

        [HttpGet]
        [Route("CommunalRepairTriageOptions")]
        public async Task<IActionResult> CommunalRepairTriageOptions(string emergencyValue,
            string notEligibleNonEmergencyValue, string unableToBookValue, string contactUsValue)
        {
            return await JourneyRepairTriageOptions(RepairType.Communal, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue, contactUsValue);
        }

        [HttpGet]
        [Route("LeaseholdRepairTriageOptions")]
        public async Task<IActionResult> LeaseholdRepairTriageOptions(string emergencyValue,
            string notEligibleNonEmergencyValue, string unableToBookValue, string contactUsValue)
        {
            return await JourneyRepairTriageOptions(RepairType.Leasehold, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue, contactUsValue);
        }

        internal async Task<IActionResult> JourneyRepairTriageOptions(string repairType, string emergencyValue,
            string notEligibleNonEmergencyValue, string unableToBookValue, string contactUsValue)
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
            if (string.IsNullOrWhiteSpace(contactUsValue))
            {
                return StatusCode(400, $"{nameof(contactUsValue)} is mandatory and must be non-empty and not whitespace");
            }

            try
            {
                var result = await retrieveJourneyTriageOptionsUseCase.Execute(repairType, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue, contactUsValue);
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
