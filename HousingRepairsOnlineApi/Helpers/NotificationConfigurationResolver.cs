using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers;

public class NotificationConfigurationResolver : INotificationConfigurationResolver
{
    private readonly IDictionary<string, INotificationConfigurationProvider> sendNotifications;

    public NotificationConfigurationResolver(IDictionary<string, INotificationConfigurationProvider> sendNotifications)
    {
        Guard.Against.Null(sendNotifications, nameof(sendNotifications));

        this.sendNotifications = sendNotifications;
    }

    public INotificationConfigurationProvider Resolve(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

        INotificationConfigurationProvider result;
        if (!sendNotifications.TryGetValue(repairType, out result))
        {
            throw new NotSupportedException($"Repair Type '{repairType}' not configured");
        }

        return result;
    }
}
