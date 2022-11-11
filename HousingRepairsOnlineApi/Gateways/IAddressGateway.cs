using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;

namespace HousingRepairsOnlineApi.Gateways
{
    public interface IAddressGateway
    {
        Task<IEnumerable<PropertyAddress>> SearchTenants(string postcode);
        Task<IEnumerable<PropertyAddress>> SearchCommunal(string postcode);
        Task<IEnumerable<PropertyAddress>> SearchLeasehold(string postcode);
    }
}
