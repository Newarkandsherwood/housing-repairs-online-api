
namespace HousingRepairsOnlineApi.Domain
{
    public class RepairRequest
    {
        public string RepairType { get; set; } /// Communal, Tenant, Leasehold
        public string Postcode { get; set; }
        public RepairAddress Address { get; set; }
        public RepairLocation Location { get; set; }
        public RepairProblem Problem { get; set; }
        public RepairIssue Issue { get; set; }
        public string ContactPersonNumber { get; set; }
        public RepairDescriptionRequest Description { get; set; }
        public RepairContactDetails ContactDetails { get; set; }
        public RepairAvailability Time { get; set; }
    }
}
