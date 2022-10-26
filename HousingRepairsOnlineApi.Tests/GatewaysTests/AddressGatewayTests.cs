using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Gateways;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests
{
    public class AddressGatewayTests : IDisposable
    {
        private const string authenticationIdentifier = "super secret";
        private const string AddressApiEndpoint = "https://our-porxy-UH.api";
        private readonly AddressGateway addressGateway;
        private readonly Mock<HttpClient> httpClientMock;
        private readonly MockHttpMessageHandler mockHttp;

        public AddressGatewayTests()
        {
            mockHttp = new MockHttpMessageHandler();

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(AddressApiEndpoint);
            addressGateway = new AddressGateway(httpClient, authenticationIdentifier);
        }

        [Fact]
        public async void ATenantAddressRequestIsMade()
        {
            // Arrange
            const string Postcode = "M3 OW";

            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/TenantAddresses?postcode={Postcode}")
                .Respond(HttpStatusCode.OK, x => new StringContent("[]"));

            // Act
            _ = await addressGateway.SearchTenants(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task TenantAddressDataFromApiIsReturned()
        {
            // Arrange
            const string Postcode = "M3 0W";

            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/TenantAddresses?postcode={Postcode}")
                .Respond("application/json",
                    "[{ \"UPRN\": \"944225244413\", " +
                    "\"Postbox\": \"null\", " +
                    "\"Room\": \"null\", " +
                    "\"Department\": \"null\", " +
                    "\"Floor\": \"null\", " +
                    "\"Plot\": \"null\", " +
                    "\"BuildingNumber\": \"123\", " +
                    "\"BuildingName\": \"null\", " +
                    "\"ComplexName\": \"null\", " +
                    "\"StreetName\": \"Cute Street\", " +
                    "\"CityName\": \"New Meow City\", " +
                    "\"AddressLine\": [\"123 Cute Street\"], " +
                    "\"Type\": \"null\", " +
                    "\"PostalCode\": \"M3 0W\"}]");
            // Act
            var data = await addressGateway.SearchTenants(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Single(data);
            Assert.Equal(Postcode, data.First().PostalCode);
        }

        [Fact]
        public async Task EmptyAddressesAreReturnedWhenTenantAddressRequestCouldNotBeMade()
        {
            // Arrange
            const string Postcode = "M3 0W";

            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/TenantAddresses?postcode={Postcode}")
                .Respond(statusCode: (HttpStatusCode)503);
            // Act
            var data = await addressGateway.SearchTenants(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Empty(data);
        }

        public void Dispose()
        {
            mockHttp.Dispose();
        }
    }
}
