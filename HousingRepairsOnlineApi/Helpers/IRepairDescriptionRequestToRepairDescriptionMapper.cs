using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IRepairDescriptionRequestToRepairDescriptionMapper
{
    public RepairDescription Map(RepairDescriptionRequest repairDescriptionRequest, string repairType);
}
