using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class RepairDurationHelper : IRepairDurationHelper
    {
        private IEnumerable<RepairDuration> repairDurations;

        public RepairDurationHelper(IEnumerable<RepairDuration> repairDurations)
        {
            Guard.Against.Null(repairDurations, nameof(repairDurations));

            this.repairDurations = repairDurations;
        }

        public int GetDaysForRepair(Repair repair)
        {
            var result = 0;
            if (repairDurations.Any())
            {
                result = repairDurations.First(r => r.Priority == repair.Priority).NumberOfDays;
            }
            return result;
        }
    }
}
