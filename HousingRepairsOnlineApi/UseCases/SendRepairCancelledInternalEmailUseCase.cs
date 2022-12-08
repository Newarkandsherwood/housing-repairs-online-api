using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

namespace HousingRepairsOnlineApi.UseCases;

public class SendRepairCancelledInternalEmailUseCase : ISendRepairCancelledInternalEmailUseCase
{
    private readonly ISendInternalEmailUseCase sendInternalEmailUseCase;
    private readonly ICancellationNotificationConfigurationProvider cancellationNotificationConfigurationProvider;

    public SendRepairCancelledInternalEmailUseCase(ISendInternalEmailUseCase sendInternalEmailUseCase,
        ICancellationNotificationConfigurationProvider cancellationNotificationConfigurationProvider)
    {
        this.sendInternalEmailUseCase = sendInternalEmailUseCase;
        this.cancellationNotificationConfigurationProvider = cancellationNotificationConfigurationProvider;
    }

    public void Execute(Repair repair)
    {
        Guard.Against.Null(repair, nameof(repair));

        var personalisationForTemplate = cancellationNotificationConfigurationProvider.GetPersonalisationForTemplate(repair);
        var templateId = cancellationNotificationConfigurationProvider.TemplateId;
        sendInternalEmailUseCase.Execute(personalisationForTemplate, templateId);
    }
}
