using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IRepairDayWindowHelper
    {
        int GetDaysForRepair(Repair repair);
    }
}
