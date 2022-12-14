using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface ISaveChangedRepairRequestUseCase
    {
        public Task Execute(Repair repair);
    }
}
