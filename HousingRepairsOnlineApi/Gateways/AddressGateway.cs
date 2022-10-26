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
        private string authenticationIdentifier;

        public AddressGateway(HttpClient httpClient, string authenticationIdentifier)
        {
            this.httpClient = httpClient;
            this.authenticationIdentifier = authenticationIdentifier;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchTenants(string postcode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"/Addresses/TenantAddresses?postcode={postcode}");

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
