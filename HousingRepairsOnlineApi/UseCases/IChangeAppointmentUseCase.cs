using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Helpers;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IChangeAppointmentUseCase
    {
        Task<ChangeAppointmentStatus> Execute(string bookingReference, DateTime startDateTime, DateTime endDateTime);
    }
}
