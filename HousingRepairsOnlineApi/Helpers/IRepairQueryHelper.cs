using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairQueryHelper
    {
        FeedIterator<Repair> GetRepairSearchIterator(string repairType, string propertyReferencem , bool includeCancelled = false);
        FeedIterator<Repair> GetRepairSearchIterator(IEnumerable<string> repairTypes, string postcode, string repairId, bool includeCancelled = false);
    }
}
