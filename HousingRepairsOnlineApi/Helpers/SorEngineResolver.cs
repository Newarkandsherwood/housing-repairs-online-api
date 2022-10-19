using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace HousingRepairsOnlineApi.Helpers;

public class SorEngineResolver : ISorEngineResolver
{
    private readonly IDictionary<string, ISoREngine> sorEngines;

    public SorEngineResolver(IDictionary<string, ISoREngine> sorEngines)
    {
        Guard.Against.Null(sorEngines, nameof(sorEngines));

        this.sorEngines = sorEngines;
    }

    public ISoREngine Resolve(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
        Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);

        ISoREngine result;
        if (!sorEngines.TryGetValue(repairType, out result))
        {
            throw new NotSupportedException($"Repair Type '{repairType}' not configured");
        }

        return result;
    }
}
