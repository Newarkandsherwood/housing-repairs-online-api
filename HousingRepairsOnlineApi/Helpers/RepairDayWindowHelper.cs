using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class RepairDayWindowHelper : IRepairDayWindowHelper
    {
        private IEnumerable<RepairDayWindow> _repairDayWindows;

        public RepairDayWindowHelper(IEnumerable<RepairDayWindow> repairDayWindows)
        {
            this._repairDayWindows = repairDayWindows;
        }

        public int GetDaysForRepair(Repair repair)
        {
            var result = 0;
            if (_repairDayWindows != null && _repairDayWindows.Any())
            {
                result = _repairDayWindows.First(r => r.Priority == repair.Priority).NumberOfDays;
            }
            return result;
        }
    }
}
