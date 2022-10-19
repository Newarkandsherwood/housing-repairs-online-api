using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISaveRepairRequestUseCase
    {
        public Task<Repair> Execute(RepairRequest repairRequest, string RepairType);
    }
}
