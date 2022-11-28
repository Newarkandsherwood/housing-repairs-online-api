using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class RepairPriorityDaysHelper : IRepairPriorityDaysHelper
    {
        private IEnumerable<RepairPriorityDays> repairPriorityDays;

        public RepairPriorityDaysHelper(IEnumerable<RepairPriorityDays> repairPriorityDays)
        {
            this.repairPriorityDays = repairPriorityDays;
        }

        public int GetDaysForRepair(Repair repair)
        {
            Guard.Against.Null(repair, nameof(repair));

            var result = 0;
            if (repairPriorityDays != null && repairPriorityDays.Any())
            {
                result = repairPriorityDays.First(r => r.Priority == repair.Priority).NumberOfDays;
            }
            return result;
        }
    }
}
