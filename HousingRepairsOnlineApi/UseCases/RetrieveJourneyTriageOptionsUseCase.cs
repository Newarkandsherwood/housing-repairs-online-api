using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveJourneyTriageOptionsUseCase : IRetrieveJourneyTriageOptionsUseCase
    {
        private readonly ISoREngine sorEngine;
        private readonly IEarlyExitRepairTriageOptionMapper earlyExitRepairTriageOptionMapper;

        public RetrieveJourneyTriageOptionsUseCase(ISoREngine sorEngine, IEarlyExitRepairTriageOptionMapper earlyExitRepairTriageOptionMapper)
        {
            this.sorEngine = sorEngine;
            this.earlyExitRepairTriageOptionMapper = earlyExitRepairTriageOptionMapper;
        }

        public Task<IEnumerable<RepairTriageOption>> Execute(string emergencyValue, string notEligibleNonEmergency, string unableToBook, string contactUsValue)
        {
            var repairTriageOptions = sorEngine.RepairTriageOptions();
            var result = earlyExitRepairTriageOptionMapper.MapRepairTriageOption(repairTriageOptions, emergencyValue,
                notEligibleNonEmergency, unableToBook, contactUsValue);

            return Task.FromResult(result);
        }
    }
}
