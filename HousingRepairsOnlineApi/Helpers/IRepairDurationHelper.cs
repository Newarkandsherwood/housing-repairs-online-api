using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairDurationHelper
    {
        int GetDaysForRepair(Repair repair);
    }
}
