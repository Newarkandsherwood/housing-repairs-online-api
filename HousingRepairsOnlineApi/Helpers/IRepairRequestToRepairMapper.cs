using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IRepairRequestToRepairMapper
{
    public Repair Map(RepairRequest repairRequest, string repairType, string repairId);
}
