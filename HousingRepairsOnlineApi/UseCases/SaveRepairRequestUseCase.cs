using System;
using System.Threading.Tasks;
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
        private readonly ISorEngineResolver sorEngineResolver;

        public SaveRepairRequestUseCase(IRepairStorageGateway cosmosGateway, IBlobStorageGateway storageGateway, ISorEngineResolver sorEngineResolver)
        {
            this.cosmosGateway = cosmosGateway;
            this.storageGateway = storageGateway;
            this.sorEngineResolver = sorEngineResolver;
        }

        public async Task<Repair> Execute(RepairRequest repairRequest)
        {
            var sorEngine = sorEngineResolver.Resolve(RepairType.Tenant);
            var repairTriageDetails = sorEngine.MapToRepairTriageDetails(
                repairRequest.Location.Value,
                repairRequest.Problem.Value,
                repairRequest.Issue?.Value);
            var repair = new Repair
            {
                Address = repairRequest.Address,
                Postcode = repairRequest.Postcode,
                Location = repairRequest.Location,
                ContactDetails = repairRequest.ContactDetails,
                Problem = repairRequest.Problem,
                Issue = repairRequest.Issue,
                ContactPersonNumber = repairRequest.ContactPersonNumber,
                Time = repairRequest.Time,
                Description = new RepairDescription
                {
                    Text = repairRequest.Description.Text,
                },
                SOR = repairTriageDetails.ScheduleOfRateCode,
                Priority = repairTriageDetails.Priority,
            };

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
