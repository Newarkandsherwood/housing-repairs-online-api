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
        private IRepairPriorityDaysHelper repairPriorityDaysHelper;

        public RepairBookingResponseHelper(IRepairPriorityDaysHelper repairPriorityDaysHelper)
        {
            this.repairPriorityDaysHelper = repairPriorityDaysHelper;
        }

        public dynamic GetRepairBookingResponse(Repair repair)
        {
            if (repair.RepairType == RepairType.Communal)
            {
                var daysForRepair = repairPriorityDaysHelper.GetDaysForRepair(repair);
                return new CommunalRepairBookingResponse() { Id = repair.Id, DaysForRepair = daysForRepair };
            }
            return new RepairBookingResponse() { Id = repair.Id };
        }
    }
}
