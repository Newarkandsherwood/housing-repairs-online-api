using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.Gateways
{
    public interface IAppointmentsGateway
    {
        Task<IEnumerable<Appointment>> GetAvailableAppointments(string sorCode, string priority, string locationId, DateTime? fromDate = null, IEnumerable<AppointmentSlotTimeSpan> allowedAppointmentSlots = default);

        Task BookAppointment(string bookingReference, string sorCode, string priority, string locationId, DateTime startDateTime, DateTime endDateTime, string repairDescriptionText);

        Task<CancelAppointmentStatus> CancelAppointment(string bookingReference);
    }
}
