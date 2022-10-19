using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers;

public class EnvironmentVariableRepairTypeSorConfigurationProvider : IRepairTypeSorConfigurationProvider
{
    public string RepairType { get; }

    public EnvironmentVariableRepairTypeSorConfigurationProvider(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), Helpers.RepairType.IsValidValue);

        RepairType = repairType;
    }

    public string ConfigurationValue()
    {
        var sorConfigurationValue = EnvironmentVariableHelper.GetEnvironmentVariable($"SOR_CONFIGURATION_{RepairType}");

        return sorConfigurationValue;
    }
}
