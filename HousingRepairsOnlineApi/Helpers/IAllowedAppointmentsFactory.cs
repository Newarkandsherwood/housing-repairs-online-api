using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IAllowedAppointmentsFactory
{
    IEnumerable<AppointmentSlotTimeSpan> CreateAllowedAppointments(string repairType);
}
