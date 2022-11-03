using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class BookAvailableAppointmentUseCase : IBookAvailableAppointmentUseCase
    {
        private readonly IAppointmentsGateway appointmentsGateway;
        private readonly IAppointmentSlotsFilter appointmentSlotsFilter;

        public BookAvailableAppointmentUseCase(IAppointmentsGateway appointmentsGateway, IAppointmentSlotsFilter appointmentSlotsFilter)
        {
            this.appointmentsGateway = appointmentsGateway;
            this.appointmentSlotsFilter = appointmentSlotsFilter;
        }

        public async Task Execute(string bookingReference, string sorCode, string priority, string locationId, string repairDescriptionText)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(priority, nameof(priority));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));
            Guard.Against.NullOrWhiteSpace(repairDescriptionText, nameof(repairDescriptionText));

            var allowedAppointmentSlotTimeSpans = appointmentSlotsFilter.Filter();

            var appointments = await appointmentsGateway.GetAvailableAppointments(sorCode, priority, locationId, null, allowedAppointmentSlotTimeSpans);
            // Need to get next available appointment
            var appointment = appointments.FirstOrDefault();
            await appointmentsGateway.BookAppointment(bookingReference, sorCode, priority, locationId, appointment.TimeOfDay.EarliestArrivalTime, appointment.TimeOfDay.LatestArrivalTime, repairDescriptionText);
        }
    }
}
