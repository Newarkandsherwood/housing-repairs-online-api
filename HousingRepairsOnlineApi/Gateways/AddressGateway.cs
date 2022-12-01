using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingRepairsOnline.Authentication.Helpers;

namespace HousingRepairsOnlineApi.Gateways
{
    public class AddressGateway : IAddressGateway
    {
        private readonly HttpClient httpClient;
        private readonly string authenticationIdentifier;

        public AddressGateway(HttpClient httpClient, string authenticationIdentifier)
        {
            this.httpClient = httpClient;
            this.authenticationIdentifier = authenticationIdentifier;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchTenants(string postcode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/Addresses/TenantAddresses?postcode={postcode}");

            return await SendRequest(request);
        }

        public async Task<IEnumerable<PropertyAddress>> SearchCommunal(string postcode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/Addresses/CommunalAddresses?postcode={postcode}");

            return await SendRequest(request);
        }

        public async Task<IEnumerable<PropertyAddress>> SearchLeasehold(string postcode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/Addresses/LeaseholdAddresses?postcode={postcode}");

            return await SendRequest(request);
        }

        private async Task<IEnumerable<PropertyAddress>> SendRequest(HttpRequestMessage request)
        {
            request.SetupJwtAuthentication(httpClient, authenticationIdentifier);

            var response = await httpClient.SendAsync(request);

            var data = new List<PropertyAddress>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<List<PropertyAddress>>();
            }

            return data;
        }
    }
}
