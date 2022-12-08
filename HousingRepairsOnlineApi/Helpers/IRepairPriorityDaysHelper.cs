using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairPriorityDaysHelper
    {
        int GetDaysForRepair(Repair repair);
    }
}
