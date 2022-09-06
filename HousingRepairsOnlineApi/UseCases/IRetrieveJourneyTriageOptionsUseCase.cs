using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IRetrieveJourneyTriageOptionsUseCase
    {
        public Task<IEnumerable<RepairTriageOption>> Execute();
    }
}
