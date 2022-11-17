using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.Helpers;
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
        private readonly string contactUsArgument = "contactUs";
        private readonly string repairTypeArgument = RepairType.Tenant;

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
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(
                x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestTenantEndpoint()
        {
            // Arrange

            // Act
            var result = await systemUnderTest.TenantRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(
                x => x.Execute(RepairType.Tenant, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestCommunalEndpoint()
        {
            // Arrange

            // Act
            var result = await systemUnderTest.CommunalRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(
                x => x.Execute(RepairType.Communal, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task TestLeaseholdEndpoint()
        {
            // Arrange

            // Act
            var result = await systemUnderTest.LeaseholdRepairTriageOptions(emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(200);
            retrieveTriageJourneyOptionsMock.Verify(
                x => x.Execute(RepairType.Leasehold, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(InvalidParameterValueData))]
        public async Task
            GivenInvalidEmergencyValueParameter_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs400(string earlyExitValue)
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, earlyExitValue, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

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
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, emergencyArgument, earlyExitValue, unableToBookArgument, contactUsArgument);

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
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, emergencyArgument, notEligibleNonEmergencyArgument, earlyExitValue, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(400);
        }

        [Theory]
        [MemberData(nameof(InvalidParameterValueData))]
        public async Task
            GivenInvalidContactIsValueParameter_WhenRequestingJourneyRepairTriageOptions_ThenStatusCodeIs400(string earlyExitValue)
        {
            // Arrange

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, earlyExitValue);

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
                    x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<System.Exception>();

            // Act
            var result = await systemUnderTest.JourneyRepairTriageOptions(repairTypeArgument, emergencyArgument, notEligibleNonEmergencyArgument, unableToBookArgument, contactUsArgument);

            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }
}
