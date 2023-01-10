using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISaveRepairRequestUseCase
    {
        public Task<Repair> Execute(string repairType, RepairRequest repairRequest, string repairId);
    }
}
