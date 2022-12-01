using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IRepairToFindRepairResponseMapper
{
    FindRepairResponse Map(Repair result);
}
