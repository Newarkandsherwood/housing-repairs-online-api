using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases;

public interface IRetrieveAvailableCommunalAppointmentUseCase
{
    public Task<AppointmentTime> Execute(string repairLocation, string repairProblem, string repairIssue,
        string locationId);
}
