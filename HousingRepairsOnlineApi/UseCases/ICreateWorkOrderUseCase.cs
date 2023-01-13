using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases;

public interface ICreateWorkOrderUseCase
{
    Task<string> Execute(string locationId, string sorCode, string description);

}
