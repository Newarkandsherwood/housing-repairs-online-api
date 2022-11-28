using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairQueryHelper
    {
        FeedIterator<Repair> GetItemQueryIterator<T>(string repairType, string propertyReference);
        FeedIterator<Repair> GetRepairSearchIterator(IEnumerable<string> repairTypes, string postcode, string repairId);
    }
}
