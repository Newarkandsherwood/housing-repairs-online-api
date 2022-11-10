using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers
{
    public interface ISendNotification
    {
        public string ConfirmationSmsTemplateId { get; set; }
        public string ConfirmationEmailTemplateId { get; set; }
        public string InternalEmailTemplateId { get; set; }
        public Dictionary<string, dynamic> GetPersonalisationForInternalEmailTemplate(Repair repair);
        public Dictionary<string, dynamic> GetPersonalisationForEmailTemplate(Repair repair);
        public Dictionary<string, dynamic> GetPersonalisationForSMSTemplate(Repair repair);
    }
}
