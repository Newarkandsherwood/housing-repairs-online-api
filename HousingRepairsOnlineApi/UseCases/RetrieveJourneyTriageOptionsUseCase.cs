using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveJourneyTriageOptionsUseCase : IRetrieveJourneyTriageOptionsUseCase
    {
        private readonly ISoREngine sorEngine;

        public RetrieveJourneyTriageOptionsUseCase(ISoREngine sorEngine)
        {
            this.sorEngine = sorEngine;
        }

        public Task<IEnumerable<RepairTriageOption>> Execute()
        {
            return Task.FromResult(sorEngine.RepairTriageOptions());
        }
    }
}
