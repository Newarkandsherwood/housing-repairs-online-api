using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public class CancelAppointmentUseCase : ICancelAppointmentUseCase
    {
        private readonly IAppointmentsGateway appointmentsGateway;

        public CancelAppointmentUseCase(IAppointmentsGateway appointmentsGateway)
        {
            this.appointmentsGateway = appointmentsGateway;
        }

        public async Task<UpdateOrCancelAppointmentStatus> Execute(string bookingReference)
        {
            Guard.Against.NullOrWhiteSpace(bookingReference, nameof(bookingReference));

            return await appointmentsGateway.CancelAppointment(bookingReference);
        }
    }
}
