using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers
{
    public class InternalEmailSender : IInternalEmailSender
    {
        private IRetrieveImageLinkUseCase retrieveImageLinkUseCase;
        private ISendInternalEmailUseCase sendInternalEmailUseCase;
        private INotificationConfigurationResolver notificationConfigurationResolver;

        public InternalEmailSender(IRetrieveImageLinkUseCase retrieveImageLinkUseCase, ISendInternalEmailUseCase sendInternalEmailUseCase, INotificationConfigurationResolver notificationConfigurationResolver)
        {
            this.retrieveImageLinkUseCase = retrieveImageLinkUseCase;
            this.sendInternalEmailUseCase = sendInternalEmailUseCase;
            this.notificationConfigurationResolver = notificationConfigurationResolver;
        }

        public async Task Execute(Repair repair)
        {
            var sendNotification = notificationConfigurationResolver.Resolve(repair.RepairType);
            var personalisation =
                await sendNotification.GetPersonalisationForInternalEmailTemplate(repair, retrieveImageLinkUseCase);
            sendInternalEmailUseCase.Execute(personalisation, sendNotification.InternalEmailTemplateId);
        }
    }
}
