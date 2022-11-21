using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests;

public class RetrieveAvailableCommunalAppointmentUseCaseTests
{
    private readonly RetrieveAvailableCommunalAppointmentUseCase systemUnderTest;
    private readonly Mock<IRetrieveAvailableAppointmentsUseCase> retrieveAvailableAppointmentsUseCaseMock;
    const string repairLocation = "kitchen";
    const string repairProblem = "cupboards";
    const string repairIssue = "doorHangingOff";
    const string locationId = "locationId";

    public RetrieveAvailableCommunalAppointmentUseCaseTests()
    {
        retrieveAvailableAppointmentsUseCaseMock = new Mock<IRetrieveAvailableAppointmentsUseCase>();
        systemUnderTest = new RetrieveAvailableCommunalAppointmentUseCase(retrieveAvailableAppointmentsUseCaseMock.Object);
    }

    [Fact]
    public void GivenNullRetrieveAvailableAppointmentsUseCaseArgument_WhenConstructing_ThenArgumentNullExceptionIsThrown()
    {
        // Arrange

        // Act
        Action act = () => _ = new RetrieveAvailableCommunalAppointmentUseCase(null);

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
        Func<Task> act = async () => await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

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
        Func<Task> act = async () => await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

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
        retrieveAvailableAppointmentsUseCaseMock.Setup(x =>
                x.Execute(RepairType.Communal, repairLocation, repairProblem, repairIssue, locationId, null))
            .ReturnsAsync(new List<AppointmentTime>());

        // Act
        Func<Task> act = async () => await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

        // Assert
        await act.Should().NotThrowAsync<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidLocationId_WhenExecute_ThenExceptionIsThrown<T>(T exception, string locationId) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange

        // Act
        Func<Task> act = async () => await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

        // Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Fact]
    public void GivenValidParameters_WhenExecuting_ThenUsesCorrectParameters()
    {
        // Arrange

        // Act
        _ = systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

        // Assert
        retrieveAvailableAppointmentsUseCaseMock.Verify(x => x.Execute(RepairType.Communal, repairLocation, repairProblem, repairIssue, locationId, null));
    }

    [Theory]
    [MemberData(nameof(MultipleAppointmentsTestData))]
    public async Task GivenMultipleAppointmentsFound_WhenExecuting_ThenReturnsEarliestAppointment(AppointmentTime expected, IEnumerable<AppointmentTime> appointmentTimes)
    {
        // Arrange
        retrieveAvailableAppointmentsUseCaseMock.Setup(x =>
                x.Execute(RepairType.Communal, repairLocation, repairProblem, repairIssue, locationId, null))
            .ReturnsAsync(appointmentTimes.ToList());

        // Act
        var actual = await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task GivenNoAppointmentsFound_WhenExecuting_ThenReturnsEmptyResult()
    {
        // Arrange
        var appointmentTimes = new List<AppointmentTime>(0);
        retrieveAvailableAppointmentsUseCaseMock.Setup(x =>
                x.Execute(RepairType.Communal, repairLocation, repairProblem, repairIssue, locationId, null))
            .ReturnsAsync(appointmentTimes);

        // Act
        var actual = await systemUnderTest.Execute(repairLocation, repairProblem, repairIssue, locationId);

        // Assert
        actual.Should().BeNull();
    }

    public static TheoryData<Exception, string> InvalidArgumentTestData() =>
        new()
        {
            { new ArgumentNullException(), null },
            { new ArgumentException(), "" },
            { new ArgumentException(), " " },
        };

    public static TheoryData<AppointmentTime, IEnumerable<AppointmentTime>> MultipleAppointmentsTestData()
    {
        var firstDate = new DateTime(2022, 11, 21);
        var firstDateMorningAppointmentTime = new AppointmentTime
        {
            StartTime = firstDate.AddHours(8),
            EndTime = firstDate.AddHours(12)
        };
        var firstDateAfternoonAppointmentTime = new AppointmentTime
        {
            StartTime = firstDate.AddHours(12),
            EndTime = firstDate.AddHours(16)
        };

        var secondDate = firstDate.AddDays(1);
        var secondDateAfternoonAppointmentTime = new AppointmentTime
        {
            StartTime = secondDate.AddHours(12),
            EndTime = secondDate.AddHours(16)
        };

        return new TheoryData<AppointmentTime, IEnumerable<AppointmentTime>>
        {
            {
                firstDateMorningAppointmentTime,
                new[]
                {
                    firstDateMorningAppointmentTime,
                    firstDateAfternoonAppointmentTime,
                }
            },
            {
                firstDateMorningAppointmentTime,
                new[]
                {
                    firstDateAfternoonAppointmentTime,
                    firstDateMorningAppointmentTime,
                }
            },
            {
                firstDateMorningAppointmentTime,
                new[]
                {
                    secondDateAfternoonAppointmentTime,
                    firstDateMorningAppointmentTime,
                }
            }
        };
    }
}
