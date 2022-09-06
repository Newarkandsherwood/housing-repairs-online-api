using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface ISoREngine
    {
        string MapSorCode(string location, string problem, string issue);

        IEnumerable<RepairTriageOption> RepairTriageOptions();
    }
}
