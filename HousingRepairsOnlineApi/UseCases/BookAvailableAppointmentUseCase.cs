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

        public async Task Execute(string bookingReference, string sorCode, string priority, string locationId, string repairDescriptionText, string repairDescriptionLocation)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(sorCode, nameof(sorCode));
            Guard.Against.NullOrWhiteSpace(priority, nameof(priority));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));
            Guard.Against.NullOrWhiteSpace(repairDescriptionText, nameof(repairDescriptionText));
            Guard.Against.NullOrWhiteSpace(repairDescriptionLocation, nameof(repairDescriptionLocation));

            var allowedAppointmentSlotTimeSpans = appointmentSlotsFilter.Filter();

            var appointments = await appointmentsGateway.GetAvailableAppointments(sorCode, priority, locationId, null, allowedAppointmentSlotTimeSpans);

            var appointment = appointments.FirstOrDefault();
            if (appointment == null)
            {
                throw new InvalidOperationException($"No appointments returned from Appointments Gateway");
            }

            var communalRepairCommentText = repairDescriptionText + " " + repairDescriptionLocation;
            if (communalRepairCommentText.Length > 255)
            {
                throw new Exception("Order comments length exceeds 255 character limit");
            }
            await appointmentsGateway.BookAppointment(bookingReference, sorCode, priority, locationId, appointment.TimeOfDay.EarliestArrivalTime, appointment.TimeOfDay.LatestArrivalTime, communalRepairCommentText);
        }
    }
}
