using System.Collections.Generic;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

public class AppointmentChangedNotificationConfigurationProvider : IAppointmentChangedNotificationConfigurationProvider
{
    public string SmsTemplateId { get; }
    public string EmailTemplateId { get; }

    public AppointmentChangedNotificationConfigurationProvider(string smsTemplateId, string emailTemplateId)
    {
        SmsTemplateId = smsTemplateId;
        EmailTemplateId = emailTemplateId;
    }

    public IDictionary<string, dynamic> GetPersonalisationForSmsTemplate(Repair repair) => GetPersonalisationForTemplate(repair);

    public IDictionary<string, dynamic> GetPersonalisationForEmailTemplate(Repair repair) => GetPersonalisationForTemplate(repair);

    private static IDictionary<string, dynamic> GetPersonalisationForTemplate(Repair repair)
    {
        Guard.Against.NullOrWhiteSpace(repair.Id, nameof(repair.Id), "The repair reference provided is invalid");
        Guard.Against.Null(repair.Time, nameof(repair.Time), "The repair appointment time provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Time.Display, $"{nameof(repair.Time)}.{nameof(repair.Time.Display)}",
            "The repair appointment time provided is invalid");

        var result = new Dictionary<string, dynamic>
        {
            { "repair_ref", repair.Id },
            { "appointment_time", repair.Time.Display },
        };

        return result;
    }
}
