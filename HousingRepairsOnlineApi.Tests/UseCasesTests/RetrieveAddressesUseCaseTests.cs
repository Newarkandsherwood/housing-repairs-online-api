using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class RetrieveAddressesUseCaseTests
    {
        private readonly RetrieveAddressesUseCase systemUnderTest;
        private readonly Mock<IAddressGateway> addressGatewayMock;

        public RetrieveAddressesUseCaseTests()
        {
            addressGatewayMock = new Mock<IAddressGateway>();
            systemUnderTest = new RetrieveAddressesUseCase(addressGatewayMock.Object);

        }

        [Fact]
        public async Task ReturnsEmptyWhenNoAddressesAreFound()
        {
            var data = await systemUnderTest.Execute("", RepairType.Tenant);
            data.Should().BeEmpty();
        }

        [Fact]
        public void SearchTenantIsCalledWhenRepairTypeIsTenantAndPostCodeIsGiven()
        {
            const string TestPostcode = "M3 0W";
            systemUnderTest.Execute(postcode: TestPostcode, RepairType.Tenant);
            addressGatewayMock.Verify(x => x.SearchTenants(TestPostcode), Times.Once);
        }

        [Fact]
        public void SearchCommunalIsCalledWhenRepairTypeIsCommunalAndPostCodeIsGiven()
        {
            const string TestPostcode = "M3 0W";
            systemUnderTest.Execute(postcode: TestPostcode, RepairType.Communal);
            addressGatewayMock.Verify(x => x.SearchCommunal(TestPostcode), Times.Once);
        }

        [Fact]
        public void SearchLeaseholdIsCalledWhenRepairTypeIsLeaseholdAndPostCodeIsGiven()
        {
            const string TestPostcode = "M3 0W";
            systemUnderTest.Execute(postcode: TestPostcode, RepairType.Leasehold);
            addressGatewayMock.Verify(x => x.SearchLeasehold(TestPostcode), Times.Once);
        }

        [Fact]
        public async Task DoesNotReturnAnEmptyCollectionOfAddresses()
        {
            const string TestPostcode = "M3 0W";
            addressGatewayMock.Setup(x => x.SearchTenants(It.IsAny<string>()))
                .ReturnsAsync(new List<PropertyAddress>());
            var data = await systemUnderTest.Execute(postcode: TestPostcode, RepairType.Tenant);
            Assert.Empty(data);
        }

        [Fact]
        public async Task ReturnACollectionOfAddresses()
        {
            const string TestPostcode = "M3 0W";
            var testAddress = new PropertyAddress()
            {
                AddressLine = new Collection<string>() { "123 cute street" },
                PostalCode = TestPostcode,
                CityName = "New Meow City"
            };
            addressGatewayMock.Setup(x => x.SearchTenants(It.IsAny<string>()))
                .ReturnsAsync(new List<PropertyAddress>() { testAddress });
            var data = await systemUnderTest.Execute(postcode: TestPostcode, RepairType.Tenant);
            Assert.Equal(data.First().AddressLine1, testAddress.AddressLine.First());
            Assert.Equal(data.First().AddressLine2, testAddress.CityName);
            Assert.Equal(data.First().PostCode, TestPostcode);
        }

        [Fact]
        public async Task GivenAnAddress_WhenExecuteIsCalled_ThenResultAddressLine1HasBuildingNumberAndAddressLine()
        {
            // Arrange
            const string TestPostcode = "M3 0W";
            var testAddress = new PropertyAddress()
            {
                BuildingNumber = "123",
                AddressLine = new Collection<string>() { "cute street" },
                PostalCode = TestPostcode,
                CityName = "New Meow City"
            };
            addressGatewayMock.Setup(x => x.SearchTenants(It.IsAny<string>()))
                .ReturnsAsync(new List<PropertyAddress>() { testAddress });

            // Act
            var data = await systemUnderTest.Execute(postcode: TestPostcode, RepairType.Tenant);

            // Assert
            var actual = data.Single();
            Assert.Equal($"123 {testAddress.AddressLine.First()}", actual.AddressLine1);
            Assert.Equal(testAddress.CityName, actual.AddressLine2);
            Assert.Equal(TestPostcode, actual.PostCode);
        }

        [Fact]
        public async void ThrowsNullExceptionWhenPostcodeIsNull()
        {
            const string TestPostcode = null;
            Func<Task> act = async () => await systemUnderTest.Execute(TestPostcode, RepairType.Tenant);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void DoesNotCallAddressGatewayWhenPostcodeIsEmpty()
        {
            const string TestPostcode = "";
            var data = systemUnderTest.Execute(postcode: TestPostcode, RepairType.Tenant);
            addressGatewayMock.Verify(x => x.SearchTenants(TestPostcode), Times.Never);
        }

        [Fact]
        public async void GivenAnAddressWithoutAnAddressLine_WhenExecuteIsCalled_ThenAnExceptionIsNotThrown()
        {
            // Arrange
            const string postCode = "LN1 3PQ";
            addressGatewayMock.Setup(x => x.SearchTenants(postCode))
                .ReturnsAsync(new List<PropertyAddress> { new() { BuildingNumber = "1", PostalCode = postCode } });

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(postCode, RepairType.Tenant);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenAnAddressWithoutAnAddressLine_WhenExecuteIsCalled_ThenAnExpectedAddressIsCreated()
        {
            // Arrange
            const string postCode = "LN1 3PQ";
            var buildingNumber = "1";
            addressGatewayMock.Setup(x => x.SearchTenants(postCode))
                .ReturnsAsync(new List<PropertyAddress> { new() { BuildingNumber = buildingNumber, PostalCode = postCode } });

            // Act
            var actual = await systemUnderTest.Execute(postCode, RepairType.Tenant);

            // Assert
            var actualAddress = actual.First();
            Assert.Equal(postCode, actualAddress.PostCode);
            Assert.Equal(buildingNumber, actualAddress.AddressLine1);
            Assert.Null(actualAddress.AddressLine2);
        }

        [Fact]
        public async void GivenAnAddressWithoutAnAddressLineAndBuildingNumber_WhenExecuteIsCalled_ThenAnExceptionIsNotThrown()
        {
            // Arrange
            const string postCode = "LN1 3PQ";
            addressGatewayMock.Setup(x => x.SearchTenants(postCode))
                .ReturnsAsync(new List<PropertyAddress> { new() { PostalCode = postCode } });

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(postCode, RepairType.Tenant);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenAnAddressWithoutAnAddressLineAndBuildingNumber_WhenExecuteIsCalled_ThenAnExpectedAddressIsCreated()
        {
            // Arrange
            const string postCode = "LN1 3PQ";
            addressGatewayMock.Setup(x => x.SearchTenants(postCode))
                .ReturnsAsync(new List<PropertyAddress> { new() { PostalCode = postCode } });

            // Act
            var actual = await systemUnderTest.Execute(postCode, RepairType.Tenant);

            // Assert
            var actualAddress = actual.Single();
            Assert.Equal(postCode, actualAddress.PostCode);
            Assert.Null(actualAddress.AddressLine1);
            Assert.Null(actualAddress.AddressLine2);
        }
    }
}
