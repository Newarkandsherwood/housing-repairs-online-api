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
        private readonly string emergencyArgument = "emergency";
        private readonly string notEligibleNonEmergencyArgument = "notEligibleNonEmergencyArgument";
        private readonly string unableToBookArgument = "unableToBookArgument";

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
            var result = await systemUnderTest.JourneyRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument);

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(
                x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(InvalidParameterValueData))]
        public async Task
            GivenInvalidEmergencyValueParameter_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs400(string earlyExitValue)
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(earlyExitValue, notEligibleNonEmergencyArgument, unableToBookArgument);

            // Assert
            GetStatusCode(result).Should().Be(400);
        }

        [Theory]
        [MemberData(nameof(InvalidParameterValueData))]
        public async Task
            GivenInvalidNotEligibleNonEmergencyValueParameter_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs400(string earlyExitValue)
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(emergencyArgument, earlyExitValue, unableToBookArgument);

            // Assert
            GetStatusCode(result).Should().Be(400);
        }

        [Theory]
        [MemberData(nameof(InvalidParameterValueData))]
        public async Task
            GivenInvalidUnableToBookValueParameter_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs400(string earlyExitValue)
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, earlyExitValue);

            // Assert
            GetStatusCode(result).Should().Be(400);
        }

        public static TheoryData<string> InvalidParameterValueData()
        {
            return new TheoryData<string> { null, string.Empty, "   ", };
        }

        [Fact]
        public async Task GivenInvalidOptions_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs500()
        {
            // Arrange
            retrieveTriageJourneyOptionsMock
                .Setup(x =>
                    x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<System.Exception>();

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument);

            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }
}
