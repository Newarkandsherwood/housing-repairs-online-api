using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.ControllersTests
{
    public class RepairTriageControllerTests : ControllerTests
    {
        private readonly RepairTriageController systemUnderTest;
        private readonly Mock<IRetrieveJourneyTriageOptionsUseCase> retrieveTriageJourneyOptionsMock;

        public RepairTriageControllerTests()
        {
            retrieveTriageJourneyOptionsMock = new Mock<IRetrieveJourneyTriageOptionsUseCase>();
            systemUnderTest = new RepairTriageController(retrieveTriageJourneyOptionsMock.Object);
        }

        [Fact]
        public async Task TestEndpoint()
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions();

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public async Task GivenInvalidOptions_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs500()
        {
            // Arrange
            retrieveTriageJourneyOptionsMock.Setup(x => x.Execute()).Throws<System.Exception>();

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions();

            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }
}
