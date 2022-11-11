using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.UseCases;

namespace HousingRepairsOnlineApi.Helpers.SendNotifications;

public class TenantNotificationConfigurationProvider: INotificationConfigurationProvider
{
    public string ConfirmationSmsTemplateId { get; set; }
    public string ConfirmationEmailTemplateId { get; set; }
    public string InternalEmailTemplateId { get; set; }

    public TenantNotificationConfigurationProvider(string confirmationSmsTemplateId, string confirmationEmailTemplateId,
        string internalEmailTemplateId)
    {
        this.ConfirmationSmsTemplateId = confirmationSmsTemplateId;
        this.ConfirmationEmailTemplateId = confirmationEmailTemplateId;
        this.InternalEmailTemplateId = internalEmailTemplateId;
    }
    public Dictionary<string, dynamic> GetPersonalisationForInternalEmailTemplate(Repair repair)
    {
        Guard.Against.NullOrWhiteSpace(repair.Id, nameof(repair.Id), "The repair reference provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Address.LocationId, nameof(repair.Address.LocationId), "The uprn provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Address.Display, nameof(repair.Address.Display), "The address provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.SOR, nameof(repair.SOR), "The sor provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Description.Text, nameof(repair.Description.Text), "The repairDescription provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.ContactDetails?.Value, nameof(repair.ContactDetails.Value), "The contact number provided is invalid");

        return new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"uprn", repair.Address.LocationId},
            {"address", repair.Address.Display},
            {"sor", repair.SOR},
            {"repair_desc", repair.Description.Text},
            {"contact_no", repair.ContactDetails?.Value}
        };
    }

    public Dictionary<string, dynamic> GetPersonalisationForEmailTemplate(Repair repair)
    {
        Guard.Against.NullOrWhiteSpace(repair.Id, nameof(repair.Id), "The booking reference provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Time.Display, nameof(repair.Time.Display), "The appointment time provided is invalid");

        return new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"appointment_time", repair.Time.Display}
        };
    }

    public Dictionary<string, dynamic> GetPersonalisationForSMSTemplate(Repair repair)
    {
        Guard.Against.NullOrWhiteSpace(repair.Id, nameof(repair.Id), "The booking reference provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Time.Display, nameof(repair.Time.Display), "The appointment time provided is invalid");

        return new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"appointment_time", repair.Time.Display}
        };
    }

    public async Task<string> GetImageLink(IRetrieveImageLinkUseCase retrieveImageLinkUseCase, Repair repair)
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
