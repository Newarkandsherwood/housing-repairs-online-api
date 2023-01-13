using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.Gateways;

public interface IWorkOrderGateway
{
    public Task<string> CreateWorkOrder(string locationId, string sorCode, string description);
}
