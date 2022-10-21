using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    using Microsoft.Azure.Cosmos;

    public class RepairQueryHelper : IRepairQueryHelper
    {
        private readonly ContainerResponse cosmosContainer;
        public RepairQueryHelper(ContainerResponse container) => cosmosContainer = container;

        public FeedIterator<Repair> GetItemQueryIterator<T>(string repairType, string propertyReference) =>
            cosmosContainer.Container.GetItemQueryIterator<Repair>(GetQueryDefinition(repairType, propertyReference));

        private static QueryDefinition GetQueryDefinition(string repairType, string propertyReference)
        {
            var query =
                "SELECT * FROM c WHERE c.Address.RepairType  = @repairType AND c.Address.LocationId  = @propertyReference ORDER BY c.Time.StartDateTime ASC";
            return new QueryDefinition(query)
                .WithParameter("@repairType", repairType)
                .WithParameter("@propertyReference", propertyReference);
       }
    }
}
