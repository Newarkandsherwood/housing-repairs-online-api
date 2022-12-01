using System;
using System.Collections.Generic;
using System.Linq;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos.Linq;

namespace HousingRepairsOnlineApi.Helpers
{
    using Microsoft.Azure.Cosmos;

    public class RepairQueryHelper : IRepairQueryHelper
    {
        private readonly ContainerResponse cosmosContainer;

        public RepairQueryHelper(ContainerResponse container) => cosmosContainer = container;

        public FeedIterator<Repair> GetItemQueryIterator<T>(string repairType, string propertyReference) =>
            cosmosContainer.Container.GetItemQueryIterator<Repair>(GetQueryDefinition(repairType, propertyReference));

        public FeedIterator<Repair> GetRepairSearchIterator(IEnumerable<string> repairTypes, string postcode, string repairId)
        {
            var repairTypesUppercase = repairTypes.Select(x => x.ToUpperInvariant());

            var query = cosmosContainer.Container.GetItemLinqQueryable<Repair>().Where(x =>
                x.Id.ToUpper() == repairId.ToUpper()
                && x.Postcode.Replace(" ", "").ToUpper() == postcode.Replace(" ", "").ToUpper()
                && repairTypesUppercase.Contains(x.RepairType.ToUpper())
                && x.Time.StartDateTime > DateTime.Today.Date
            );

            var result = query.ToFeedIterator();

            return result;
        }

        private static QueryDefinition GetQueryDefinition(string repairType, string propertyReference)
        {
            var query =
                "SELECT * FROM c WHERE UPPER(c.RepairType) = UPPER(@repairType) AND c.Address.LocationId = @propertyReference ORDER BY c.Time.StartDateTime ASC";
            return new QueryDefinition(query)
                .WithParameter("@repairType", repairType)
                .WithParameter("@propertyReference", propertyReference);
        }
    }
}
