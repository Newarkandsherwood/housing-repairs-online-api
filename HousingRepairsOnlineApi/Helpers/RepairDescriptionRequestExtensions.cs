using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public static class RepairDescriptionRequestExtensions
{
    public static string CombinedDescriptionTexts(this RepairDescriptionRequest repairDescription)
    {
        if (repairDescription.LocationText != null)
        {
            return repairDescription.LocationText + " " + repairDescription.Text;
        }

        return repairDescription.Text;
    }
}
