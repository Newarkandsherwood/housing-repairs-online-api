using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

public abstract class BaseNotificationConfigurationProvider
{
    public string ConfirmationSmsTemplateId { get; init; }
    public string ConfirmationEmailTemplateId { get; init; }
    public string InternalEmailTemplateId { get; init; }

    protected BaseNotificationConfigurationProvider(string confirmationSmsTemplateId, string confirmationEmailTemplateId,
        string internalEmailTemplateId)
    {
        ConfirmationSmsTemplateId = confirmationSmsTemplateId;
        ConfirmationEmailTemplateId = confirmationEmailTemplateId;
        InternalEmailTemplateId = internalEmailTemplateId;
    }
}
