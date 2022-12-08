using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ICancelRepairRequestUseCase
    {
        public Task Execute(Repair repair);
    }
}
