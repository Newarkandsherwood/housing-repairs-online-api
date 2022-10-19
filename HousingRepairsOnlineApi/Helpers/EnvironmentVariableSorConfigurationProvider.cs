namespace HousingRepairsOnlineApi.Helpers
{
    public class EnvironmentVariableSorConfigurationProvider : ISorConfigurationProvider
    {
        public string ConfigurationValue()
        {
            var sorConfigurationValue = EnvironmentVariableHelper.GetEnvironmentVariable("SOR_CONFIGURATION_TENANT");

            return sorConfigurationValue;
        }
    }
}
