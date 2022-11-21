﻿using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers
{
    public class AppointmentConfirmationSender : IAppointmentConfirmationSender
    {
        private readonly ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase;
        private readonly ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase;
        private readonly INotificationConfigurationResolver notificationConfigurationResolver;

        public AppointmentConfirmationSender(ISendAppointmentConfirmationEmailUseCase sendAppointmentConfirmationEmailUseCase, ISendAppointmentConfirmationSmsUseCase sendAppointmentConfirmationSmsUseCase, INotificationConfigurationResolver notificationConfigurationResolver)
        {
            this.sendAppointmentConfirmationEmailUseCase = sendAppointmentConfirmationEmailUseCase;
            this.sendAppointmentConfirmationSmsUseCase = sendAppointmentConfirmationSmsUseCase;
            this.notificationConfigurationResolver = notificationConfigurationResolver;
        }

        public void Execute(Repair repair)
        {
            var sendNotification = notificationConfigurationResolver.Resolve(repair.RepairType);
            switch (repair?.ContactDetails?.Type)
            {
                case AppointmentConfirmationSendingTypes.Email:
                    sendAppointmentConfirmationEmailUseCase.Execute(repair.ContactDetails.Value, sendNotification.GetPersonalisationForEmailTemplate(repair), sendNotification.ConfirmationEmailTemplateId);
                    break;
                case AppointmentConfirmationSendingTypes.Sms:
                    sendAppointmentConfirmationSmsUseCase.Execute(repair.ContactDetails.Value, sendNotification.GetPersonalisationForSmsTemplate(repair), sendNotification.ConfirmationSmsTemplateId);
                    break;
                default:
                    throw new Exception($"Invalid contact type: {repair?.ContactDetails?.Type}");
            }
        }
    }
}
