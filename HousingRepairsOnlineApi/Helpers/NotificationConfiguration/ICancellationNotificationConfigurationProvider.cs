using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

public interface ICancellationNotificationConfigurationProvider
{
    string TemplateId { get; }

    IDictionary<string, dynamic> GetPersonalisationForTemplate(Repair repair);
}
