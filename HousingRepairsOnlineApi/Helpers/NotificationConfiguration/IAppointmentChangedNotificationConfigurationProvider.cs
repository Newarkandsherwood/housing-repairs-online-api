using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

public interface IAppointmentChangedNotificationConfigurationProvider
{
    string SmsTemplateId { get; }
    string EmailTemplateId { get; }

    IDictionary<string, dynamic> GetPersonalisationForSmsTemplate(Repair repair);
    IDictionary<string, dynamic> GetPersonalisationForEmailTemplate(Repair repair);
}
