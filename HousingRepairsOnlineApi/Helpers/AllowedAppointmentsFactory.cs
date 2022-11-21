using System.Collections.Generic;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Helpers;

public class AllowedAppointmentsFactory : IAllowedAppointmentsFactory
{
    private readonly IEnumerable<AppointmentSlotTimeSpan> allAllowedAppointmentSlots;
    private readonly IDictionary<string, IAppointmentSlotsFilter> appointmentSlotsFilterByRepairType;

    public AllowedAppointmentsFactory(IEnumerable<AppointmentSlotTimeSpan> allAllowedAppointmentSlots,
        IDictionary<string, IAppointmentSlotsFilter> appointmentSlotsFilterByRepairType)
    {
        Guard.Against.Null(appointmentSlotsFilterByRepairType, nameof(appointmentSlotsFilterByRepairType));
        Guard.Against.Null(allAllowedAppointmentSlots, nameof(allAllowedAppointmentSlots));

        this.appointmentSlotsFilterByRepairType = appointmentSlotsFilterByRepairType;
        this.allAllowedAppointmentSlots = allAllowedAppointmentSlots;
    }

    public IEnumerable<AppointmentSlotTimeSpan> CreateAllowedAppointments(string repairType)
    {
        Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));

        var result = allAllowedAppointmentSlots;
        if (appointmentSlotsFilterByRepairType.TryGetValue(repairType, out var appointmentSlotsFilter))
        {
            result = appointmentSlotsFilter.Filter(allAllowedAppointmentSlots);
        }


        return result;
    }
}
