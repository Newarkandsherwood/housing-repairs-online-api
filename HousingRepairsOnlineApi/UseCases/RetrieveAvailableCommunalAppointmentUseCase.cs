using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases;

public class RetrieveAvailableCommunalAppointmentUseCase : IRetrieveAvailableCommunalAppointmentUseCase
{
    private readonly IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase;

    public RetrieveAvailableCommunalAppointmentUseCase(IRetrieveAvailableAppointmentsUseCase retrieveAvailableAppointmentsUseCase)
    {
        Guard.Against.Null(retrieveAvailableAppointmentsUseCase, nameof(retrieveAvailableAppointmentsUseCase));

        this.retrieveAvailableAppointmentsUseCase = retrieveAvailableAppointmentsUseCase;
    }

    public async Task<AppointmentTime> Execute(string repairLocation, string repairProblem, string repairIssue, string locationId)
    {
        Guard.Against.NullOrWhiteSpace(repairLocation, nameof(repairLocation));
        Guard.Against.NullOrWhiteSpace(repairProblem, nameof(repairProblem));
        Guard.Against.NullOrWhiteSpace(locationId, nameof(locationId));

        var appointments = await retrieveAvailableAppointmentsUseCase.Execute(RepairType.Communal,
            repairLocation, repairProblem,
            repairIssue, locationId);

        var result = appointments.MinBy(x => x.StartTime);

        return result;
    }
}
