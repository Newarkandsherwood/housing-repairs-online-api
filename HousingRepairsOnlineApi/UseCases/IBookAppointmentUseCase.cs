using System;
using System.Threading.Tasks;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IBookAppointmentUseCase
    {
        Task Execute(string bookingReference, string sorCode, string priority, string locationId, DateTime startDateTime,
            DateTime endDateTime, string repairDescriptionText);
    }
}
