using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class RetrieveAvailableAppointmentsUseCaseTests
    {
        private readonly RetrieveAvailableAppointmentsUseCase systemUnderTest;
        private readonly Mock<IAppointmentsGateway> appointmentsGatewayMock;
        private readonly Mock<ISorEngineResolver> sorEngineResolverMock;
        private readonly Mock<ISoREngine> sorEngineMock;
        const string repairType = RepairType.Tenant;
        const string kitchen = "kitchen";
        const string cupboards = "cupboards";
        const string doorHangingOff = "doorHangingOff";

        public RetrieveAvailableAppointmentsUseCaseTests()
        {
            sorEngineMock = new Mock<ISoREngine>();
            sorEngineResolverMock = new Mock<ISorEngineResolver>();
            sorEngineResolverMock.Setup(x => x.Resolve(It.IsAny<string>())).Returns(sorEngineMock.Object);
            appointmentsGatewayMock = new Mock<IAppointmentsGateway>();
            systemUnderTest = new RetrieveAvailableAppointmentsUseCase(appointmentsGatewayMock.Object, sorEngineResolverMock.Object);
        }

        [Fact]
        public void GivenNullAppointmentsGateway_WhenConstructing_ThenArgumentNullExceptionIsThrown()
        {
            // Arrange

            // Act
            Action act = () => new RetrieveAvailableAppointmentsUseCase(null, sorEngineResolverMock.Object, allowedAppointmentsFactoryMock.Object);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void GivenNullSorEngineResolver_WhenConstructing_ThenArgumentNullExceptionIsThrown()
        {
            // Arrange

            // Act
            Action act = () => new RetrieveAvailableAppointmentsUseCase(new Mock<IAppointmentsGateway>().Object, null, allowedAppointmentsFactoryMock.Object);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidRepairLocation_WhenExecute_ThenExceptionIsThrown<T>(T exception, string repairLocation) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairType, repairLocation, cupboards, doorHangingOff, "A UPRN");

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidRepairProblem_WhenExecute_ThenExceptionIsThrown<T>(T exception, string repairProblem) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairType, kitchen, repairProblem, doorHangingOff, "A UPRN");

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenValidRepairIssue_WhenExecute_ThenNoExceptionIsThrown<T>(T exception, string repairIssue) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairType, kitchen, cupboards, repairIssue, "A UPRN");

            // Assert
            await act.Should().NotThrowAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidUprn_WhenExecute_ThenExceptionIsThrown<T>(T exception, string uprn) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairType, kitchen, cupboards, doorHangingOff, uprn);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(Helpers.RepairTypeTestData.InvalidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidRepairType_WhenExecute_ThenExceptionIsThrown<T>(T exception, string repairTypeParameter) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairTypeParameter, kitchen, cupboards, doorHangingOff, "locationId");

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Theory]
        [MemberData(nameof(Helpers.RepairTypeTestData.ValidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
        public void GivenValidRepairTypeParameter_WhenResolving_ThenExceptionIsNotThrown(string repairTypeParameter)
        {
            // Arrange

            // Act
            Action act = () => _ = systemUnderTest.Execute(repairTypeParameter, kitchen, cupboards, doorHangingOff, "locationId");

            // Assert
            act.Should().NotThrow<ArgumentException>();
        }

        [Fact]
#pragma warning disable xUnit1026
        public async void GivenANullFromDate_WhenExecute_ThenExceptionIsNotThrown()
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairType, kitchen, cupboards, doorHangingOff, "Location ID", null);

            // Assert
            await act.Should().NotThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenRepairParameters_WhenExecute_ThenMapToRepairTriageDetails()
        {
            sorEngineMock.Setup(x => x.MapToRepairTriageDetails(kitchen, cupboards, doorHangingOff))
                .Returns(new RepairTriageDetails());
            await systemUnderTest.Execute(repairType, kitchen, cupboards, doorHangingOff, "uprn");
            sorEngineMock.Verify(x => x.MapToRepairTriageDetails(kitchen, cupboards, doorHangingOff), Times.Once);
        }

        [Fact]
        public async void GivenRepairParameters_WhenExecute_ThenGetAvailableAppointmentsGatewayIsCalled()
        {
            var repairCode = "N373049";
            var priority = "priority";
            var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = repairCode, Priority = priority };
            sorEngineMock.Setup(x => x.MapToRepairTriageDetails(kitchen, cupboards, doorHangingOff)).Returns(repairTriageDetails);
            await systemUnderTest.Execute(repairType, kitchen, cupboards, doorHangingOff, "uprn");
            appointmentsGatewayMock.Verify(x => x.GetAvailableAppointments(repairCode, priority, "uprn", null, null), Times.Once);
        }

        [Fact]
        public async void GivenRepairParameters_WhenExecute_AppointmentTimeAreReturned()
        {
            var repairCode = "N373049";
            var priority = "priority";
            var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = repairCode, Priority = priority };
            var startTime = DateTime.Today.AddHours(8);
            var endTime = DateTime.Today.AddHours(12);

            sorEngineMock.Setup(x => x.MapToRepairTriageDetails(kitchen, cupboards, doorHangingOff)).Returns(repairTriageDetails);

            appointmentsGatewayMock.Setup(x => x.GetAvailableAppointments(repairCode, priority, "uprn", null, null))
                .ReturnsAsync(new List<Appointment> { new()
                {
                    TimeOfDay = new TimeOfDay
                    {
                        EarliestArrivalTime = startTime,
                        LatestArrivalTime = endTime
                    },
                } });

            var actual = await systemUnderTest.Execute(repairType, kitchen, cupboards, doorHangingOff, "uprn");
            var actualAddress = actual.First();

            Assert.Equal(startTime, actualAddress.StartTime);
            Assert.Equal(endTime, actualAddress.EndTime);
        }

    }
}
