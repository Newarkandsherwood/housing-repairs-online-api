using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class RepairBookingResponseHelper : IRepairBookingResponseHelper
    {
        private IRepairDayWindowHelper repairDayWindowHelper;

        public RepairBookingResponseHelper(IRepairDayWindowHelper repairDayWindowHelper)
        {
            this.repairDayWindowHelper = repairDayWindowHelper;
        }

        public dynamic GetRepairBookingResponse(Repair repair)
        {
            if (repair.RepairType == RepairType.Communal)
            {
                var daysForRepair = repairDayWindowHelper.GetDaysForRepair(repair);
                return new RepairCommunalBookingResponse() { Id = repair.Id, DaysForRepair = daysForRepair };
            }
            return new RepairBookingResponse() { Id = repair.Id };
        }
    }
}
