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
        private ISendNotificationResolver sendNotificationResolver;

        public InternalEmailSender(IRetrieveImageLinkUseCase retrieveImageLinkUseCase, ISendInternalEmailUseCase sendInternalEmailUseCase, ISendNotificationResolver sendNotificationResolver)
        {
            this.retrieveImageLinkUseCase = retrieveImageLinkUseCase;
            this.sendInternalEmailUseCase = sendInternalEmailUseCase;
            this.sendNotificationResolver = sendNotificationResolver;
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
            var sendNotification = sendNotificationResolver.Resolve(repair.RepairType);
            sendInternalEmailUseCase.Execute(sendNotification.GetPersonalisationForInternalEmailTemplate(repair), imageLink, sendNotification.InternalEmailTemplateId);
        }
    }
}
