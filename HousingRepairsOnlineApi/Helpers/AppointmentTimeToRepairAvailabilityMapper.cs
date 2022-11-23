using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class AppointmentTimeToRepairAvailabilityMapper : IAppointmentTimeToRepairAvailabilityMapper
{
    public RepairAvailability Map(AppointmentTime appointmentTime)
    {
        Guard.Against.Null(appointmentTime, nameof(appointmentTime));

        return new RepairAvailability
        {
            StartDateTime = appointmentTime.StartTime,
            EndDateTime = appointmentTime.EndTime,
        };
    }
}
