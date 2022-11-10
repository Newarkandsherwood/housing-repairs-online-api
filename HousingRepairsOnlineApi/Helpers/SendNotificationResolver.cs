using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers;

public class SendNotificationResolver : ISendNotificationResolver
{
    private readonly IDictionary<string, ISendNotification> sendNotifications;

    public SendNotificationResolver(IDictionary<string, ISendNotification> sendNotifications)
    {
        Guard.Against.Null(sendNotifications, nameof(sendNotifications));

        this.sendNotifications = sendNotifications;
    }

    public ISendNotification Resolve(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

        ISendNotification result;
        if (!sendNotifications.TryGetValue(repairType, out result))
        {
            throw new NotSupportedException($"Repair Type '{repairType}' not configured");
        }

        return result;
    }
}
