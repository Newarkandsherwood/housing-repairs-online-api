using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Controllers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.ControllersTests
{
    public class AppointmentsControllerTests : ControllerTests
    {
        private AppointmentsController systemUndertest;
        private Mock<IRetrieveAvailableAppointmentsUseCase> availableAppointmentsUseCaseMock;
        public AppointmentsControllerTests()
        {
            availableAppointmentsUseCaseMock = new Mock<IRetrieveAvailableAppointmentsUseCase>();
            systemUndertest = new AppointmentsController(availableAppointmentsUseCaseMock.Object);
        }

        [Fact]
        public async Task TestEndpoint()
        {
            const string RepairLocation = "kitchen";
            const string RepairProblem = "cupboards";
            const string RepairIssue = "doorHangingOff";

            const string Uprn = "12345";
            var result = await systemUndertest.AvailableAppointments(RepairLocation, RepairProblem, RepairIssue, Uprn);
            GetStatusCode(result).Should().Be(200);
            availableAppointmentsUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), RepairLocation, RepairProblem, RepairIssue, Uprn, null), Times.Once);
        }

        [Fact]
        public async Task GivenAFromDate_WhenRequestingAvailableAppointments_ThenResultsAreReturned()
        {
            // Arrange
            const string RepairLocation = "kitchen";
            const string RepairProblem = "cupboards";
            const string RepairIssue = "doorHangingOff";
            const string LocationId = "location ID";
            var fromDate = new DateTime(2021, 12, 15);

            // Act
            var result = await systemUndertest.AvailableAppointments(RepairLocation, RepairProblem, RepairIssue, LocationId);

            // Assert
            GetStatusCode(result).Should().Be(200);
            availableAppointmentsUseCaseMock.Verify(x => x.Execute(It.IsAny<string>(), RepairLocation, RepairProblem, RepairIssue, LocationId, null), Times.Once);
        }

        [Fact]
        public async Task ReturnsErrorWhenFailsToSave()
        {
            const string RepairLocation = "kitchen";
            const string RepairProblem = "cupboards";
            const string RepairIssue = "doorHangingOff";
            const string LocationId = "location ID";
            var fromDate = new DateTime(2021, 12, 15);

            availableAppointmentsUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Throws<Exception>();

            var result = await systemUndertest.AvailableAppointments(RepairLocation, RepairProblem, RepairIssue, LocationId, fromDate);

            GetStatusCode(result).Should().Be(500);
        }
    }
}
