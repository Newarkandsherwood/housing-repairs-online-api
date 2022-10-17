using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface ISoREngine
    {
        RepairTriageDetails MapSorCode(string location, string problem, string issue);

        IEnumerable<RepairTriageOption> RepairTriageOptions();
    }
}
