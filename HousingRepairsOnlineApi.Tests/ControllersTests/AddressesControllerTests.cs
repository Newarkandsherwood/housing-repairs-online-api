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
        private AddressesController systemUnderTest;
        private Mock<IRetrieveAddressesUseCase> retrieveAddressesUseCaseMock;
        private const string TenantRepairType = RepairType.Tenant;

        public AddressesControllerTests()
        {
            retrieveAddressesUseCaseMock = new Mock<IRetrieveAddressesUseCase>();
            systemUnderTest = new AddressesController(retrieveAddressesUseCaseMock.Object);
        }

        [Fact]
        public async Task TestEndpoint()
        {
            const string Postcode = "M3 0W";
            var result = await systemUnderTest.GetTenantAddresses(Postcode);

            GetStatusCode(result).Should().Be(200);
            retrieveAddressesUseCaseMock.Verify(x => x.Execute(Postcode, TenantRepairType), Times.Once);
        }

        [Fact]
        public async Task ReturnsErrorWhenFailsToSave()
        {
            const string Postcode = "M3 0W";

            retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>())).Throws<System.Exception>();

            var result = await systemUnderTest.GetTenantAddresses(Postcode);

            GetStatusCode(result).Should().Be(500);
        }
    }
}
