using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairToRepairBookingResponseMapper
    {
        dynamic MapRepairBookingResponse(Repair repair, bool includeDays);
    }
}
