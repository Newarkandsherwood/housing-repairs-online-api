using System.Collections.Generic;
using System.Linq;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class LargestAppointmentSlotFilter : IAppointmentSlotsFilter
{
    private readonly IEnumerable<AppointmentSlotTimeSpan> appointmentSlots;

    public LargestAppointmentSlotFilter(IEnumerable<AppointmentSlotTimeSpan> appointmentSlots)
    {
        this.appointmentSlots = appointmentSlots;
    }

    public IEnumerable<AppointmentSlotTimeSpan> Filter()
    {
        return appointmentSlots.Count() <= 1 ? appointmentSlots : FindLargestAppointmentSlot(appointmentSlots);
    }

    private IEnumerable<AppointmentSlotTimeSpan> FindLargestAppointmentSlot(IEnumerable<AppointmentSlotTimeSpan> appointmentSlotTimeSpans)
    {
        var result = appointmentSlotTimeSpans.OrderByDescending(x => x.EndTime - x.StartTime).Take(1);

        return result;
    }
}
