using System;
using System.Linq;

namespace HousingRepairsOnlineApi.Helpers;

public static class RepairType
{
    public const string Tenant = "TENANT";

    public static readonly string[] All = { Tenant };

    public static Func<string, bool> IsValidValue = repairType => All.Contains(repairType);
}
