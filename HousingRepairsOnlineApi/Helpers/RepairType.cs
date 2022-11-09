using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRepairsOnlineApi.Helpers;

public static class RepairType
{
    public const string Tenant = "TENANT";

    public const string Communal = "COMMUNAL";

    public static string Leasehold = "LEASEHOLD";

    public static readonly IEnumerable<string> All = new[] { Tenant, Communal, Leasehold };

    public static Func<string, bool> IsValidValue = repairType => All.Contains(repairType);
}
