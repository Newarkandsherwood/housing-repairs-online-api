using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public static class RepairExtensions
{
    public static string AppendLocationDescription(this RepairDescription repairDescription)
    {
        if (repairDescription.LocationText != null)
        {
            return repairDescription.LocationText + " " + repairDescription.Text;
        }

        return repairDescription.Text;
    }
}
