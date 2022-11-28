using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairToRepairBookingResponseMapper
    {
        dynamic GetRepairBookingResponse(Repair repair, bool includeDays);
    }
}
