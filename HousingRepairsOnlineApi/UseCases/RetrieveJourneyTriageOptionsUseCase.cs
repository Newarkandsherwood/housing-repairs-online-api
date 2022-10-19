using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveJourneyTriageOptionsUseCase : IRetrieveJourneyTriageOptionsUseCase
    {
        private readonly ISorEngineResolver sorEngineResolver;
        private readonly IEarlyExitRepairTriageOptionMapper earlyExitRepairTriageOptionMapper;

        public RetrieveJourneyTriageOptionsUseCase(ISorEngineResolver sorEngineResolver, IEarlyExitRepairTriageOptionMapper earlyExitRepairTriageOptionMapper)
        {
            this.sorEngineResolver = sorEngineResolver;
            this.earlyExitRepairTriageOptionMapper = earlyExitRepairTriageOptionMapper;
        }

        public Task<IEnumerable<RepairTriageOption>> Execute(string emergencyValue, string notEligibleNonEmergency, string unableToBook, string contactUsValue)
        {
            var sorEngine = sorEngineResolver.Resolve(RepairType.Tenant);
            var repairTriageOptions = sorEngine.RepairTriageOptions();
            var result = earlyExitRepairTriageOptionMapper.MapRepairTriageOption(repairTriageOptions, emergencyValue,
                notEligibleNonEmergency, unableToBook, contactUsValue);

            return Task.FromResult(result);
        }
    }
}
