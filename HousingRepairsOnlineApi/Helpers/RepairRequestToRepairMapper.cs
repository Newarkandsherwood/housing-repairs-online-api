using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class RepairRequestToRepairMapper : IRepairRequestToRepairMapper
{
    private readonly ISorEngineResolver sorEngineResolver;

    private readonly IRepairDescriptionRequestToRepairDescriptionMapper
        repairDescriptionRequestToRepairDescriptionMapper;

    public RepairRequestToRepairMapper(ISorEngineResolver sorEngineResolver, IRepairDescriptionRequestToRepairDescriptionMapper repairDescriptionRequestToRepairDescriptionMapper)
    {
        this.sorEngineResolver = sorEngineResolver;
        this.repairDescriptionRequestToRepairDescriptionMapper = repairDescriptionRequestToRepairDescriptionMapper;
    }
    public Repair Map(RepairRequest repairRequest, string repairType)
    {
        var sorEngine = sorEngineResolver.Resolve(repairType);
        var repairTriageDetails = sorEngine.MapToRepairTriageDetails(
            repairRequest.Location.Value,
            repairRequest.Problem.Value,
            repairRequest.Issue?.Value);

        var repair = new Repair
        {
            RepairType = repairType,
            Address = repairRequest.Address,
            Postcode = repairRequest.Postcode,
            Location = repairRequest.Location,
            ContactDetails = repairRequest.ContactDetails,
            Problem = repairRequest.Problem,
            Issue = repairRequest.Issue,
            ContactPersonNumber = repairRequest.ContactPersonNumber,
            Time = repairRequest.Time,
            Description = repairDescriptionRequestToRepairDescriptionMapper.Map(repairRequest.Description, repairType),
            SOR = repairTriageDetails.ScheduleOfRateCode,
            Priority = repairTriageDetails.Priority,
        };

        return repair;
    }
}
