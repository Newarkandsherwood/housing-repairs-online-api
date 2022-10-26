using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;

namespace HousingRepairsOnlineApi.UseCases
{
    public interface IRetrieveAddressesUseCase
    {
        Task<IEnumerable<Address>> Execute(string postcode, string repairType);
    }
}
