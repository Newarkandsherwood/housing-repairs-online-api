using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers;

public class NotificationConfigurationResolver : INotificationConfigurationResolver
{
    private readonly IDictionary<string, INotificationConfigurationProvider> notificationConfigurationProviders;

    public NotificationConfigurationResolver(IDictionary<string, INotificationConfigurationProvider> notificationConfigurationProviders)
    {
        Guard.Against.Null(notificationConfigurationProviders, nameof(notificationConfigurationProviders));

        this.notificationConfigurationProviders = notificationConfigurationProviders;
    }

    public INotificationConfigurationProvider Resolve(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

        INotificationConfigurationProvider result;
        if (!notificationConfigurationProviders.TryGetValue(repairType, out result))
        {
            throw new NotSupportedException($"Repair Type '{repairType}' not configured");
        }

        return result;
    }
}
