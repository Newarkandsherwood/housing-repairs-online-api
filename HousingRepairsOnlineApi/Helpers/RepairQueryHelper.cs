using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos.Linq;

namespace HousingRepairsOnlineApi.Helpers
{
    using Microsoft.Azure.Cosmos;

    public class RepairQueryHelper : IRepairQueryHelper
    {
        private readonly ContainerResponse cosmosContainer;

        private readonly Expression<Func<Repair, bool>> isFutureRepair = x => x.Time.StartDateTime > DateTime.Today.Date;

        public RepairQueryHelper(ContainerResponse container) => cosmosContainer = container;

        public FeedIterator<Repair> GetRepairSearchIterator(string repairType, string propertyReference)
        {
            var query = cosmosContainer.Container.GetItemLinqQueryable<Repair>()
                .Where(x =>
                    x.RepairType.ToUpper() == repairType.ToUpper()
                    && x.Address.LocationId == propertyReference)
                .Where(isFutureRepair)
                .OrderBy(x => x.Time.StartDateTime);

            var result = query.ToFeedIterator();

            return result;
        }

        public FeedIterator<Repair> GetRepairSearchIterator(IEnumerable<string> repairTypes, string postcode, string repairId)
        {
            var repairTypesUppercase = repairTypes.Select(x => x.ToUpperInvariant());

            var query = cosmosContainer.Container.GetItemLinqQueryable<Repair>().Where(x =>
                    x.Id.ToUpper() == repairId.ToUpper()
                    && x.Postcode.Replace(" ", "").ToUpper() == postcode.Replace(" ", "").ToUpper()
                    && repairTypesUppercase.Contains(x.RepairType.ToUpper()))
                .Where(isFutureRepair)
                .OrderBy(x => x.Time.StartDateTime);

            var result = query.ToFeedIterator();

            return result;
        }
    }
}
