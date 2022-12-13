using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class ChangeAppointmentUseCase : IChangeAppointmentUseCase
    {
        private readonly IAppointmentsGateway appointmentsGateway;

        public ChangeAppointmentUseCase(IAppointmentsGateway appointmentsGateway)
        {
            this.appointmentsGateway = appointmentsGateway;
        }

        public async Task<UpdateOrCancelAppointmentStatus> Execute(string bookingReference, DateTime startDateTime, DateTime endDateTime)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(bookingReference));
            Guard.Against.OutOfRange(endDateTime, nameof(endDateTime), startDateTime, DateTime.MaxValue);

            return await appointmentsGateway.ChangeAppointment(bookingReference, startDateTime, endDateTime);
        }
    }
}
