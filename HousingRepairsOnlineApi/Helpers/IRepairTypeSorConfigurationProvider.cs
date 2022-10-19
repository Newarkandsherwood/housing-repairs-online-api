namespace HousingRepairsOnlineApi.Helpers;

public interface IRepairTypeSorConfigurationProvider : ISorConfigurationProvider
{
    public string RepairType { get; }
}
