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
            var imageLink = "None";
            if (!String.IsNullOrEmpty(repair.Description?.PhotoUrl))
            {
                await Task.Run(() =>
                {
                    imageLink = retrieveImageLinkUseCase.Execute(repair.Description?.PhotoUrl);
                });
            }
            var sendNotification = notificationConfigurationResolver.Resolve(repair.RepairType);
            sendInternalEmailUseCase.Execute(sendNotification.GetPersonalisationForInternalEmailTemplate(repair), sendNotification.GetImageLink(retrieveImageLinkUseCase, repair), sendNotification.InternalEmailTemplateId);
        }
    }
}
