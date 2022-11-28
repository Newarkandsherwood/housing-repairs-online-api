using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class RepairToRepairBookingResponseMapper : IRepairToRepairBookingResponseMapper
    {
        private IRepairPriorityDaysHelper repairPriorityDaysHelper;

        public RepairToRepairBookingResponseMapper(IRepairPriorityDaysHelper repairPriorityDaysHelper)
        {
            this.repairPriorityDaysHelper = repairPriorityDaysHelper;
        }

        public dynamic MapRepairBookingResponse(Repair repair, bool includeDays)
        {
            if (includeDays)
            {
                var daysForRepair = repairPriorityDaysHelper.GetDaysForRepair(repair);
                return new RepairBookingResponseWithDays() { Id = repair.Id, DaysForRepair = daysForRepair };
            }
            return new RepairBookingResponse() { Id = repair.Id };
        }
    }
}
