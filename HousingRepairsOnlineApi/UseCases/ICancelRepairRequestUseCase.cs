using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ICancelRepairRequestUseCase
    {
        public Task Execute(string repairId);
    }
}
