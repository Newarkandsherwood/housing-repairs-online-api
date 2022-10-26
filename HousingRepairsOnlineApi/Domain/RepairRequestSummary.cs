
namespace HousingRepairsOnlineApi.Domain
{
    public class RepairRequestSummary
    {
        public string Postcode { get; set; }
        public string SOR { get; set; }
        public RepairAddress Address { get; set; }
        public RepairLocation Location { get; set; }
        public RepairProblem Problem { get; set; }
        public RepairIssue Issue { get; set; }
        public RepairDescription Description { get; set; }
    }
}
