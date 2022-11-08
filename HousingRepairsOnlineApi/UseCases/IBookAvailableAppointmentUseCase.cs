using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IBookAvailableAppointmentUseCase
    {
        Task Execute(string bookingReference, string sorCode, string priority, string locationId, string repairDescriptionText);
    }
}
