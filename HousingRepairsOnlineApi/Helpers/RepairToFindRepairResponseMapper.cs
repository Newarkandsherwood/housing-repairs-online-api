using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class RepairToFindRepairResponseMapper : IRepairToFindRepairResponseMapper
{
    public FindRepairResponse Map(Repair result) =>
        new()
        {
            Address = result.Address,
            Location = result.Location,
            Problem = result.Problem,
            Issue = result.Issue,
            AppointmentTime = result.Time,
        };
}
