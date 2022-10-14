using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IRetrieveRepairsUseCase
    {
        Task<IEnumerable<Repair>> Execute(string propertyReference);
    }
}
