using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Gateways;

namespace HousingRepairsOnlineApi.UseCases
{
    public class BookAvailableAppointmentUseCase : IBookAvailableAppointmentUseCase
    {
        private readonly IAppointmentsGateway appointmentsGateway;

        public BookAvailableAppointmentUseCase(IAppointmentsGateway appointmentsGateway)
        {
            this.appointmentsGateway = appointmentsGateway;
        }

        public async Task Execute(string bookingReference, string sorCode, string priority, string locationId, DateTime startDateTime,
            DateTime endDateTime, string repairDescriptionText)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(priority, nameof(priority));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));
            Guard.Against.NullOrWhiteSpace(repairDescriptionText, nameof(repairDescriptionText));

            var appointments = await appointmentsGateway.GetAvailableAppointments(sorCode, priority, locationId, null, null);
            // Need to get next available appointment
            var appointment = appointments.FirstOrDefault();
            await appointmentsGateway.BookAppointment(bookingReference, sorCode, priority, locationId, appointment.Date.Value, appointment.Date.Value, repairDescriptionText);
        }
    }
}
