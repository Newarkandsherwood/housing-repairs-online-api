using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    using Microsoft.Azure.Cosmos;

    public class RepairQueryHelper : IRepairQueryHelper
    {
        private readonly Container cosmosContainer;
        public RepairQueryHelper(Container container) => cosmosContainer = container;

        public FeedIterator<Repair> GetItemQueryIterator<T>(string propertyReference) =>
            cosmosContainer.GetItemQueryIterator<Repair>(GetQueryDefinition(propertyReference));

        private static QueryDefinition GetQueryDefinition(string propertyReference)
        {
            var query =
                "SELECT * FROM c WHERE c.Address.LocationId  = @propertyReference ORDER BY c.Time.StartDateTime ASC";
            return new QueryDefinition(query)
                .WithParameter("@propertyReference", propertyReference);
        }
    }
}
