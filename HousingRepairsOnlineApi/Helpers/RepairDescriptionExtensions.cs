using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public static class RepairExtensions
{
    public static string CombinedDescriptionTexts(this RepairDescription repairDescription)
    {
        if (repairDescription.LocationText != null)
        {
            return repairDescription.LocationText + " " + repairDescription.Text;
        }

        return repairDescription.Text;
    }
}
