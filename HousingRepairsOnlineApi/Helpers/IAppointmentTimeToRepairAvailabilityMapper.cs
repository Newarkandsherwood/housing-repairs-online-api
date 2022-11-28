using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IAppointmentTimeToRepairAvailabilityMapper
{
    RepairAvailability Map(AppointmentTime appointmentTime);
}
