using System.Collections.Generic;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers.NotificationConfiguration;

public class CancellationInternalNotificationConfigurationProvider : ICancellationNotificationConfigurationProvider
{
    public CancellationInternalNotificationConfigurationProvider(string templateId)
    {
        TemplateId = templateId;
    }

    public string TemplateId { get; }

    public IDictionary<string, dynamic> GetPersonalisationForTemplate(Repair repair)
    {
        Guard.Against.NullOrWhiteSpace(repair.Id, nameof(repair.Id), "The repair reference provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Address.LocationId, nameof(repair.Address.LocationId), "The uprn provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Address.Display, nameof(repair.Address.Display), "The address provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.SOR, nameof(repair.SOR), "The sor provided is invalid");
        Guard.Against.NullOrWhiteSpace(repair.Description.Text, nameof(repair.Description.Text), "The repairDescription provided is invalid");

        var result = new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"uprn", repair.Address.LocationId},
            {"address", repair.Address.Display},
            {"sor", repair.SOR},
            {"repair_desc", repair.Description.Text},
        };

        return result;
    }
}
