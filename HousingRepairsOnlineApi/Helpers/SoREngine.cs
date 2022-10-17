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

        public RepairTriageDetails MapSorCode(string location, string problem, string issue)
        {
            RepairTriageDetails result;
            if (issue is null)
            {
                result = sorMapping[location][problem];
                return result;
            }

            result = sorMapping[location][problem][issue];

            return result;
        }

        public IEnumerable<RepairTriageOption> RepairTriageOptions()
        {
            return journeyTriageOptions;
        }
    }
}
