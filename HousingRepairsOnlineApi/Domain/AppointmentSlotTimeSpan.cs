using System;

namespace HousingRepairsOnlineApi.Domain;

public class AppointmentSlotTimeSpan
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
