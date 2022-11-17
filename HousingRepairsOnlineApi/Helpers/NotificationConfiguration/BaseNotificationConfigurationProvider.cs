using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers.SendNotifications;

public class BaseNotificationConfigurationProvider
{
    public string ConfirmationSmsTemplateId { get; set; }
    public string ConfirmationEmailTemplateId { get; set; }
    public string InternalEmailTemplateId { get; set; }

    protected BaseNotificationConfigurationProvider(string confirmationSmsTemplateId, string confirmationEmailTemplateId,
        string internalEmailTemplateId)
    {
        ConfirmationSmsTemplateId = confirmationSmsTemplateId;
        ConfirmationEmailTemplateId = confirmationEmailTemplateId;
        InternalEmailTemplateId = internalEmailTemplateId;
    }

    protected async Task<string> GetImageLink(Repair repair, IRetrieveImageLinkUseCase retrieveImageLinkUseCase)
    {
        var imageLink = "None";
        if (!string.IsNullOrEmpty(repair.Description?.PhotoUrl))
        {
            await Task.Run(() =>
            {
                imageLink = retrieveImageLinkUseCase.Execute(repair.Description?.PhotoUrl);
            });
        }
        return imageLink;
    }
}
