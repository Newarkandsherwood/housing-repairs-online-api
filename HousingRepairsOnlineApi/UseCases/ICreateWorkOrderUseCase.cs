using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases;

public interface ICreateWorkOrderUseCase
{
    Task<string> Execute(string repairType, RepairRequest repairRequest);
}
