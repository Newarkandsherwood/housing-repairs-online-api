using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class CancelRepairRequestUseCase : ICancelRepairRequestUseCase
    {
        private readonly IRepairStorageGateway cosmosGateway;

        public CancelRepairRequestUseCase(IRepairStorageGateway cosmosGateway)
        {
            this.cosmosGateway = cosmosGateway;
        }

        public async Task Execute(string repairId)
        {
            Guard.Against.NullOrWhiteSpace(repairId, nameof(repairId));
            await cosmosGateway.CancelRepair(repairId);
        }
    }
}
