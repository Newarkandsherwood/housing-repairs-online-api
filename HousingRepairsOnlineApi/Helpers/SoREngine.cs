using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class SoREngine : ISoREngine
    {
        private readonly IDictionary<string, IDictionary<string, dynamic>> sorMapping;
        private readonly IEnumerable<RepairTriageOption> journeyTriageOptions;

        public SoREngine(IDictionary<string, IDictionary<string, dynamic>> sorMapping, IEnumerable<RepairTriageOption> journeyTriageOptions)
        {
            this.sorMapping = sorMapping;
            this.journeyTriageOptions = journeyTriageOptions;
        }

        public RepairTriageDetails MapToRepairTriageDetails(string location, string problem, string issue)
        {
            return issue is null ? sorMapping[location][problem] : sorMapping[location][problem][issue];
        }

        public IEnumerable<RepairTriageOption> RepairTriageOptions()
        {
            return journeyTriageOptions;
        }
    }
}
