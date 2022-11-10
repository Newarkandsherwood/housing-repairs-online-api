using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers
{
    public class AppointmentConfirmationSender : IAppointmentConfirmationSender
    {
        private readonly ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase;
        private readonly ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase;
        private readonly ISendNotificationResolver sendNotificationResolver;

        public AppointmentConfirmationSender(ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase, ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase, ISendNotificationResolver sendNotificationResolver)
        {
            this.sendAppointmentConfirmationEmailUseCase = sendAppointmentConfirmationEmailUseCase;
            this.sendAppointmentConfirmationSmsUseCase = sendAppointmentConfirmationSmsUseCase;
            this.sendNotificationResolver = sendNotificationResolver;
        }

        public void Execute(Repair repair)
        {
            var sendNotification = sendNotificationResolver.Resolve(repair.RepairType);
            switch (repair?.ContactDetails?.Type)
            {
                case AppointmentConfirmationSendingTypes.Email:
                    sendAppointmentConfirmationEmailUseCase.Execute(repair.ContactDetails.Value, sendNotification.GetPersonalisationForEmailTemplate(repair), sendNotification.ConfirmationEmailTemplateId);
                    break;
                case AppointmentConfirmationSendingTypes.Sms:
                    sendAppointmentConfirmationSmsUseCase.Execute(repair.ContactDetails.Value, sendNotification.GetPersonalisationForSMSTemplate(repair), sendNotification.ConfirmationSmsTemplateId);
                    break;
                default:
                    throw new Exception($"Invalid contact type: {repair?.ContactDetails?.Type}");
            }
        }
    }
}
