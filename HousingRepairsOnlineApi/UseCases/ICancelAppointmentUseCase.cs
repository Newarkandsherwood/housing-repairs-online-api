using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ICancelAppointmentUseCase
    {
        Task<CancelAppointmentStatus> Execute(string bookingReference);
    }
}
