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
        private const string AuthenticationIdentifier = "super secret";
        private const string AddressApiEndpoint = "https://our-porxy-UH.api";
        private const string Postcode = "M3 0W";
        private readonly AddressGateway addressGateway;
        private readonly Mock<HttpClient> httpClientMock;
        private readonly MockHttpMessageHandler mockHttp;

        public AddressGatewayTests()
        {
            mockHttp = new MockHttpMessageHandler();

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(AddressApiEndpoint);
            addressGateway = new AddressGateway(httpClient, AuthenticationIdentifier);
        }

        [Fact]
        public async void ATenantAddressRequestIsMade()
        {
            // Arrange
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
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/TenantAddresses?postcode={Postcode}")
                .Respond(statusCode: (HttpStatusCode)503);
            // Act
            var data = await addressGateway.SearchTenants(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Empty(data);
        }

        [Fact]
        public async void ACommunalAddressRequestIsMade()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/CommunalAddresses?postcode={Postcode}")
                .Respond(HttpStatusCode.OK, x => new StringContent("[]"));

            // Act
            _ = await addressGateway.SearchCommunal(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task CommunalAddressDataFromApiIsReturned()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/CommunalAddresses?postcode={Postcode}")
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
            var data = await addressGateway.SearchCommunal(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Single(data);
            Assert.Equal(Postcode, data.First().PostalCode);
        }

        [Fact]
        public async Task EmptyAddressesAreReturnedWhenCommunalAddressRequestCouldNotBeMade()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/CommunalAddresses?postcode={Postcode}")
                .Respond(statusCode: (HttpStatusCode)503);
            // Act
            var data = await addressGateway.SearchCommunal(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Empty(data);
        }

        [Fact]
        public async void ALeaseholdAddressRequestIsMade()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/LeaseholdAddresses?postcode={Postcode}")
                .Respond(HttpStatusCode.OK, x => new StringContent("[]"));

            // Act
            _ = await addressGateway.SearchLeasehold(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task LeaseholdAddressDataFromApiIsReturned()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/LeaseholdAddresses?postcode={Postcode}")
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
            var data = await addressGateway.SearchLeasehold(Postcode);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            Assert.Single(data);
            Assert.Equal(Postcode, data.First().PostalCode);
        }

        [Fact]
        public async Task EmptyAddressesAreReturnedWhenLeaseholdAddressRequestCouldNotBeMade()
        {
            // Arrange
            mockHttp.Expect($"{AddressApiEndpoint}/Addresses/LeaseholdAddresses?postcode={Postcode}")
                .Respond(statusCode: (HttpStatusCode)503);
            // Act
            var data = await addressGateway.SearchLeasehold(Postcode);

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
