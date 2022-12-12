using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IChangeRepairRequestUseCase
    {
        public Task Execute(Repair repair,  DateTime startDateTime, DateTime endDateTime);
    }
}
