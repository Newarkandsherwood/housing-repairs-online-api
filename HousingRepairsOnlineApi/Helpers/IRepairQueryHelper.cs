using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairQueryHelper
    {
        FeedIterator<Repair> GetItemQueryIterator<T>(string propertyReference) ;
    }
}
