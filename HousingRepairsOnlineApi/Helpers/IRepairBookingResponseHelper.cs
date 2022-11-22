using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairBookingResponseHelper
    {
        dynamic GetRepairBookingResponse(Repair repair);
    }
}
