using System.Collections.Generic;
using System.Linq;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class LargestAppointmentSlotFilter : IAppointmentSlotsFilter
{
    public IEnumerable<AppointmentSlotTimeSpan> Filter(IEnumerable<AppointmentSlotTimeSpan> appointmentSlotTimeSpans)
    {
        var result = appointmentSlotTimeSpans.OrderByDescending(x => x.EndTime - x.StartTime).Take(1);

        return result;
    }
}
