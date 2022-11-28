using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Azure.Cosmos;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairToRepairBookingResponseMapper
    {
        RepairBookingResponse MapRepairBookingResponse(Repair repair, bool includeDays);
    }
}
