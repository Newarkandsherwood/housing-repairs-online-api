namespace HousingRepairsOnlineApi.Helpers
{
    public static class EarlyExitValues
    {
        public const string EmergencyExitValue = "Emergency";
        public const string UnableToBook = "UnableToBook";
        public const string NotEligibleNonEmergency = "NotEligibleNonEmergency";

        public static readonly string[] All = { EmergencyExitValue, UnableToBook, NotEligibleNonEmergency };
    }
}
