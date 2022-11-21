using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using ApplicationTime = HousingRepairsOnlineApi.Domain.AppointmentTime;

namespace HousingRepairsOnlineApi.UseCases
{
    public class RetrieveAvailableAppointmentsUseCase : IRetrieveAvailableAppointmentsUseCase
    {
        private readonly IAppointmentsGateway appointmentsGateway;
        private readonly ISorEngineResolver sorEngineResolver;
        private readonly IEnumerable<AppointmentSlotTimeSpan> allowedAppointmentSlots;

        public RetrieveAvailableAppointmentsUseCase(IAppointmentsGateway appointmentsGateway, ISorEngineResolver sorEngineResolver, IEnumerable<AppointmentSlotTimeSpan> allowedAppointmentSlots = default)
        {
            Guard.Against.Null(appointmentsGateway, nameof(appointmentsGateway));
            Guard.Against.Null(sorEngineResolver, nameof(sorEngineResolver));

            this.appointmentsGateway = appointmentsGateway;
            this.sorEngineResolver = sorEngineResolver;
            this.allowedAppointmentSlots = allowedAppointmentSlots;
        }

        public async Task<List<ApplicationTime>> Execute(string repairType, string repairLocation, string repairProblem,
            string repairIssue, string locationId, DateTime? fromDate = null)
        {
            Guard.Against.NullOrWhiteSpace(repairType, nameof(repairType));
            Guard.Against.InvalidInput(repairType, nameof(repairType), RepairType.IsValidValue);
            Guard.Against.NullOrWhiteSpace(repairLocation, nameof(repairLocation));
            Guard.Against.NullOrWhiteSpace(repairProblem, nameof(repairProblem));
            Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));

            var sorEngine = sorEngineResolver.Resolve(repairType);
            var repairCode = sorEngine.MapToRepairTriageDetails(repairLocation, repairProblem, repairIssue);

            var result = await appointmentsGateway.GetAvailableAppointments(repairCode.ScheduleOfRateCode, repairCode.Priority, locationId, fromDate, allowedAppointmentSlots);
            var convertedResults = result.Select(ConvertToHactAppointment).ToList();

            return convertedResults;

            ApplicationTime ConvertToHactAppointment(Appointment appointment)
            {

                return new ApplicationTime
                {
                    StartTime = appointment.TimeOfDay.EarliestArrivalTime,
                    EndTime = appointment.TimeOfDay.LatestArrivalTime
                };
            }
        }
    }
}
