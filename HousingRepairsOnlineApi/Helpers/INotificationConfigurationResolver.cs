namespace HousingRepairsOnlineApi.Helpers;

public interface INotificationConfigurationResolver
{
    INotificationConfigurationProvider Resolve(string repairType);
}
