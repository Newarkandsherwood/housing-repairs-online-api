using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface IEarlyExitRepairTriageOptionMapper
    {
        IEnumerable<RepairTriageOption> MapRepairTriageOption(IEnumerable<RepairTriageOption> repairTriageOptions,
            string emergencyValue, string notEligibleNonEmergencyValue, string unableToBookValue);
    }
}
