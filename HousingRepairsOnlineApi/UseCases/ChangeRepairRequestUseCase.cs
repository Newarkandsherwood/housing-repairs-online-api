using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class ChangeRepairRequestUseCase : IChangeRepairRequestUseCase
    {
        private readonly IRepairStorageGateway repairStorageGateway;

        public ChangeRepairRequestUseCase(IRepairStorageGateway repairStorageGateway)
        {
            this.repairStorageGateway = repairStorageGateway;
        }

        public async Task Execute(Repair repair, DateTime startDateTime, DateTime endDateTime)
        {
            Guard.Against.Null(repair, nameof(repair));
            repair.Time.StartDateTime = startDateTime;
            repair.Time.EndDateTime = endDateTime;
            await repairStorageGateway.ModifyRepair(repair);
        }
    }
}
