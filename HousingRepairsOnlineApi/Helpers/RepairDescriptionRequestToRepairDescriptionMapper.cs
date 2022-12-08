using System;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class RepairDescriptionRequestToRepairDescriptionMapper : IRepairDescriptionRequestToRepairDescriptionMapper
{
    public RepairDescription Map(RepairDescriptionRequest repairRequest, string repairType)
    {
        var repairDescription = new RepairDescription { Text = repairRequest.Text };

        switch (repairType)
        {
            case RepairType.Communal:
                {
                    repairDescription.Text = repairRequest.LocationText + " " + repairDescription.Text;
                    break;
                }
        }
        return repairDescription;
    }
}
