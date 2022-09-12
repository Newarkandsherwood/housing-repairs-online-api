using System.Collections.Generic;
using System.Linq;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public class EarlyExitRepairTriageOptionMapper : IEarlyExitRepairTriageOptionMapper
    {
        public IEnumerable<RepairTriageOption> MapRepairTriageOption(
            IEnumerable<RepairTriageOption> repairTriageOptions, string emergencyValue,
            string notEligibleNonEmergencyValue, string unableToBookValue)
        {
            var earlyExitValuesMapped = new Dictionary<string, string>
            {
                { EarlyExitValues.EmergencyExitValue, emergencyValue },
                { EarlyExitValues.NotEligibleNonEmergency, notEligibleNonEmergencyValue },
                { EarlyExitValues.UnableToBook, unableToBookValue },
            };

            var result = repairTriageOptions.Select(x => new RepairTriageOption
            {
                Display = x.Display,
                Value = earlyExitValuesMapped.ContainsKey(x.Value) ? earlyExitValuesMapped[x.Value] : x.Value,
                Options = x.Options == null
                    ? null
                    : MapRepairTriageOption(x.Options, emergencyValue, notEligibleNonEmergencyValue,
                        unableToBookValue).ToArray(),
            });

            return result;
        }
    }
}
