using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ICancelAppointmentUseCase
    {
        Task<UpdateOrCancelAppointmentStatus> Execute(string bookingReference);
    }
}
