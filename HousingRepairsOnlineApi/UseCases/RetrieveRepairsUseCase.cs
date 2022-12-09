using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Extensions.Logging;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveRepairsUseCase : IRetrieveRepairsUseCase
    {
        private readonly IRepairStorageGateway cosmosGateway;

        public RetrieveRepairsUseCase(IRepairStorageGateway cosmosGateway)
        {
            this.cosmosGateway = cosmosGateway;
        }

        public async Task<IEnumerable<RepairRequestSummary>> Execute(string repairType, string propertyReference)
        {
            // Retrieve the repairs from the Cosmos DB for this
            if (propertyReference == null)
                throw new ArgumentNullException(nameof(propertyReference));

            return await cosmosGateway.SearchByPropertyReference(repairType, propertyReference);
        }

        public async Task<Repair> Execute(IEnumerable<string> repairTypes, string postcode, string repairId, bool includeCancelled = false)
        {
            Guard.Against.NullOrEmpty(repairTypes, nameof(repairTypes));
            Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));
            Guard.Against.NullOrWhiteSpace(repairId, nameof(repairId));

            var searchTask = cosmosGateway.SearchByPostcodeAndId(repairTypes, postcode, repairId, includeCancelled);

            var result = await searchTask.ContinueWith(x =>
            {
                var repairRequestSummaries = x.Result.ToArray();
                return repairRequestSummaries.Length == 1 ? repairRequestSummaries[0] : null;
            });

            return result;
        }
    }
}
