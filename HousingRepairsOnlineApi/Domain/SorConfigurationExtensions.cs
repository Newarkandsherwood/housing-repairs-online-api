using System.Linq;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.Domain
{
    public static class SorConfigurationExtensions
    {
        public static bool IsValid(this SorConfiguration sorConfiguration)
        {
            var result = IsEarlyExit();
            if (!result)
            {
                result = HasEitherSorCodeOrOptions();
                if (result && sorConfiguration.Options != null)
                {
                    result = HasAtLeastOneOptionAndAllOptionsAreValid();
                }
            }

            return result;

            bool IsEarlyExit()
            {
                return sorConfiguration.Options == null && sorConfiguration.SorCode == null &&
                       EarlyExitValues.All.Contains(sorConfiguration.Value);
            }

            bool HasEitherSorCodeOrOptions()
            {
                return (sorConfiguration.Options != null) ^ (sorConfiguration.SorCode != null);
            }

            bool HasAtLeastOneOptionAndAllOptionsAreValid()
            {
                return sorConfiguration.Options.Any() && sorConfiguration.Options.All(x => x.IsValid());
            }
        }
    }
}
