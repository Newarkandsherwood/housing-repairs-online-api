using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests
{
    public class AddressesControllerTests : ControllerTests
    {
        private readonly AddressesController systemUnderTest;
        private readonly Mock<IRetrieveAddressesUseCase> retrieveAddressesUseCaseMock;
        private const string Postcode = "M3 0W";

        public AddressesControllerTests()
        {
            retrieveAddressesUseCaseMock = new Mock<IRetrieveAddressesUseCase>();
            systemUnderTest = new AddressesController(retrieveAddressesUseCaseMock.Object);
        }

        [Fact]
        public async Task TestGetTenantEndpoint()
        {
            var result = await systemUnderTest.GetTenantAddresses(Postcode);

            GetStatusCode(result).Should().Be(200);
            retrieveAddressesUseCaseMock.Verify(x => x.Execute(Postcode, RepairType.Tenant), Times.Once);
        }

        [Fact]
        public async Task TestCommunalEndpoint()
        {
            var result = await systemUnderTest.GetCommunalAddresses(Postcode);

            GetStatusCode(result).Should().Be(200);
            retrieveAddressesUseCaseMock.Verify(x => x.Execute(Postcode, RepairType.Communal), Times.Once);
        }

        [Fact]
        public async Task TestGetLeaseholdEndpoint()
        {
            var result = await systemUnderTest.GetLeaseholdAddresses(Postcode);

            GetStatusCode(result).Should().Be(200);
            retrieveAddressesUseCaseMock.Verify(x => x.Execute(Postcode, RepairType.Leasehold), Times.Once);
        }


        [Fact]
        public async Task ReturnsErrorWhenFailsToSave()
        {
            retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>())).Throws<System.Exception>();

            var result = await systemUnderTest.GetTenantAddresses(Postcode);

            GetStatusCode(result).Should().Be(500);
        }
    }
}
