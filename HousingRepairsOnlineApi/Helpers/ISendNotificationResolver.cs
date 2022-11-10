namespace HousingRepairsOnlineApi.Helpers;

public interface ISendNotificationResolver
{
    ISendNotification Resolve(string repairType);
}
