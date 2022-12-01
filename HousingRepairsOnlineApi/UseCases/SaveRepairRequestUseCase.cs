using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Extensions.Logging;

namespace HousingRepairsOnlineApi.UseCases
{
    public class SaveRepairRequestUseCase : ISaveRepairRequestUseCase
    {
        private readonly IRepairStorageGateway cosmosGateway;
        private readonly IBlobStorageGateway storageGateway;
        private readonly IRepairRequestToRepairMapper repairRequestToRepairMapper;

        public SaveRepairRequestUseCase(IRepairStorageGateway cosmosGateway, IBlobStorageGateway storageGateway, IRepairRequestToRepairMapper repairRequestToRepairMapper)
        {
            this.cosmosGateway = cosmosGateway;
            this.storageGateway = storageGateway;
            this.repairRequestToRepairMapper = repairRequestToRepairMapper;
        }

        public async Task<Repair> Execute(string repairType, RepairRequest repairRequest)
        {
            Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
            Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

            var repair = repairRequestToRepairMapper.Map(repairRequest, repairType);

            if (!string.IsNullOrEmpty(repairRequest.Description.Base64Img))
            {
                var photoUrl = storageGateway.UploadBlob(
                    repairRequest.Description.Base64Img,
                    repairRequest.Description.FileExtension
                ).Result;
                repair.Description.PhotoUrl = photoUrl;

            }

            var savedRequest = await cosmosGateway.AddRepair(repair);

            return savedRequest;
        }
    }
}
