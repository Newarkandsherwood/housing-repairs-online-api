using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.Gateways
{
    public interface IAppointmentsGateway
    {
        Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string locationId, DateTime? fromDate = null, IEnumerable<AppointmentSlotTimeSpan> allowedAppointmentSlots = default);

        Task BookAppointment(string bookingReference, string sorCode, string locationId, DateTime startDateTime, DateTime endDateTime);
    }
}
