using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
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

        public Task<IEnumerable<RepairTriageOption>> Execute(string repairType, string emergencyValue, string notEligibleNonEmergency, string unableToBook, string contactUsValue)
        {
            Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
            Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

            var sorEngine = sorEngineResolver.Resolve(repairType);
            var repairTriageOptions = sorEngine.RepairTriageOptions();
            var result = earlyExitRepairTriageOptionMapper.MapRepairTriageOption(repairTriageOptions, emergencyValue,
                notEligibleNonEmergency, unableToBook, contactUsValue);

            return Task.FromResult(result);
        }
    }
}
