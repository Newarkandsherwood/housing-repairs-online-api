using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class RepairRequestToRepairMapper : IRepairRequestToRepairMapper
{
    private readonly ISorEngineResolver sorEngineResolver;

    public RepairRequestToRepairMapper(ISorEngineResolver sorEngineResolver)
    {
        this.sorEngineResolver = sorEngineResolver;
    }
    public Repair Map(RepairRequest repairRequest, string repairType, string repairId)
    {
        var sorEngine = sorEngineResolver.Resolve(repairType);
        var repairTriageDetails = sorEngine.MapToRepairTriageDetails(
            repairRequest.Location.Value,
            repairRequest.Problem.Value,
            repairRequest.Issue?.Value);

        var repair = new Repair
        {
            Id = repairId,
            RepairType = repairType,
            Address = repairRequest.Address,
            Postcode = repairRequest.Postcode,
            Location = repairRequest.Location,
            ContactDetails = repairRequest.ContactDetails,
            Problem = repairRequest.Problem,
            Issue = repairRequest.Issue,
            ContactPersonNumber = repairRequest.ContactPersonNumber,
            Time = repairRequest.Time,
            Description = new RepairDescription
            {
                LocationText = repairRequest.Description.LocationText,
                Text = repairRequest.Description.Text,
                Base64Image = repairRequest.Description.Base64Img,
            },
            SOR = repairTriageDetails.ScheduleOfRateCode,
            Priority = repairTriageDetails.Priority,
        };

        return repair;
    }
}
