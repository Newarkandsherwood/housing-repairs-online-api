using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class CancelRepairRequestUseCase : ICancelRepairRequestUseCase
    {
        private readonly IRepairStorageGateway repairStorageGateway;

        public CancelRepairRequestUseCase(IRepairStorageGateway repairStorageGateway)
        {
            this.repairStorageGateway = repairStorageGateway;
        }

        public async Task Execute(Repair repair)
        {
            Guard.Against.Null(repair, nameof(repair));
            await repairStorageGateway.CancelRepair(repair);
        }
    }
}
