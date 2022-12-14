using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class SaveChangedRepairRequestUseCase : ISaveChangedRepairRequestUseCase
    {
        private readonly IRepairStorageGateway repairStorageGateway;

        public SaveChangedRepairRequestUseCase(IRepairStorageGateway repairStorageGateway)
        {
            this.repairStorageGateway = repairStorageGateway;
        }

        public async Task Execute(Repair repair)
        {
            Guard.Against.Null(repair, nameof(repair));
            await repairStorageGateway.ModifyRepair(repair);
        }
    }
}
