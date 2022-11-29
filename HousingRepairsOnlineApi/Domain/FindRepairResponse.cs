namespace HousingRepairsOnlineApi.Domain;

public class FindRepairResponse
{
    public RepairAddress Address { get; set; }
    public RepairLocation Location { get; set; }
    public RepairProblem Problem { get; set; }
    public RepairIssue Issue { get; set; }
    public RepairAvailability AppointmentTime { get; set; }
}
