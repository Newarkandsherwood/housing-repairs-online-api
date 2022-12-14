using System;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

namespace HousingRepairsOnlineApi.UseCases;

public class SendRepairAppointmentChangedNotificationUseCase : ISendRepairAppointmentChangedNotificationUseCase
{
    private readonly ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase;
    private readonly ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase;
    private readonly IAppointmentChangedNotificationConfigurationProvider appointmentChangedNotificationConfigurationProvider;

    public SendRepairAppointmentChangedNotificationUseCase(
        ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase,
        ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase,
        IAppointmentChangedNotificationConfigurationProvider appointmentChangedNotificationConfigurationProvider)
    {
        this.sendAppointmentConfirmationEmailUseCase = sendAppointmentConfirmationEmailUseCase;
        this.sendAppointmentConfirmationSmsUseCase = sendAppointmentConfirmationSmsUseCase;
        this.appointmentChangedNotificationConfigurationProvider = appointmentChangedNotificationConfigurationProvider;
    }

    public void Execute(Repair repair)
    {
        Guard.Against.Null(repair, nameof(repair));

        var repairContactDetails = repair.ContactDetails;
        switch (repairContactDetails.Type)
        {
            case AppointmentConfirmationSendingTypes.Email:
                {
                    var personalisationForEmailTemplate =
                        appointmentChangedNotificationConfigurationProvider.GetPersonalisationForEmailTemplate(repair);
                    var emailTemplateId = appointmentChangedNotificationConfigurationProvider.EmailTemplateId;

                    sendAppointmentConfirmationEmailUseCase.Execute(repairContactDetails.Value,
                        personalisationForEmailTemplate, emailTemplateId);
                    break;
                }
            case AppointmentConfirmationSendingTypes.Sms:
                {
                    var personalisationForSmsTemplate =
                        appointmentChangedNotificationConfigurationProvider.GetPersonalisationForSmsTemplate(repair);
                    var smsTemplateId = appointmentChangedNotificationConfigurationProvider.SmsTemplateId;

                    sendAppointmentConfirmationSmsUseCase.Execute(repairContactDetails.Value, personalisationForSmsTemplate,
                        smsTemplateId);
                    break;
                }
            default:
                throw new NotSupportedException($"Contact type '{repairContactDetails.Type}' is not supported");
        }
    }
}
