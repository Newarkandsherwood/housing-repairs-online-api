using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ICancelAppointmentUseCase
    {
        Task<ChangeAppointmentStatus> Execute(string bookingReference);
    }
}
