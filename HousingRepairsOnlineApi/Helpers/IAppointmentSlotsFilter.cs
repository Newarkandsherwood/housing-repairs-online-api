using System.Collections.Generic;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public interface IAppointmentSlotsFilter
{
    IEnumerable<AppointmentSlotTimeSpan> Filter();
}
