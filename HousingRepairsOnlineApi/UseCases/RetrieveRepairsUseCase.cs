using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            if(propertyReference == null)
                throw new ArgumentNullException(nameof(propertyReference));

            return await cosmosGateway.SearchByPropertyReference(repairType, propertyReference);
        }
    }
}
