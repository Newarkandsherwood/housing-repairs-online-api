namespace HousingRepairsOnlineApi.Helpers;

public enum UpdateOrCancelAppointmentStatus
{
    Unknown = 0,
    NotFound = 1,
    Error = 2,
    AppointmentUpdated = 100,
    AppointmentCancelled = 101
}
