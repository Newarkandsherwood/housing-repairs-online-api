using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface INotificationConfigurationProvider
    {
        public string ConfirmationSmsTemplateId { get; }
        public string ConfirmationEmailTemplateId { get; }
        public string InternalEmailTemplateId { get; }
        public Task<Dictionary<string, dynamic>> GetPersonalisationForInternalEmailTemplate(Repair repair, IRetrieveImageLinkUseCase retrieveImageLinkUseCase);
        public Dictionary<string, dynamic> GetPersonalisationForEmailTemplate(Repair repair);
        public Dictionary<string, dynamic> GetPersonalisationForSmsTemplate(Repair repair);
    }
}
