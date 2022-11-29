using System;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class RepairToFindRepairResponseMapperTests
{
    private RepairToFindRepairResponseMapper systemUnderTest = new();

    [Fact]
    public void GivenRepairWithAddress_WhenMapping_ThenResponseHasAddress()
    {
        // Arrange
        var repairAddress = new RepairAddress
        {
            Display = "1 Address Street, Address City, AB1 2CD",
            LocationId = "locationId",
        };
        var repair = new Repair
        {
            Address = repairAddress
        };

        var expected = new FindRepairResponse
        {
            Address = repairAddress
        };

        // Act
        var actual = systemUnderTest.Map(repair);

        // Assert
        actual.Address.Should().BeEquivalentTo(expected.Address);
    }

    [Fact]
    public void GivenRepairWithLocation_WhenMapping_ThenResponseHasAddress()
    {
        // Arrange
        var repairLocation = new RepairLocation
        {
            Display = "Kitchen",
            Value = "kitchen",
        };
        var repair = new Repair
        {
            Location = repairLocation
        };

        var expected = new FindRepairResponse
        {
            Location = repairLocation
        };

        // Act
        var actual = systemUnderTest.Map(repair);

        // Assert
        actual.Location.Should().BeEquivalentTo(expected.Location);
    }

    [Fact]
    public void GivenRepairWithProblem_WhenMapping_ThenResponseHasAddress()
    {
        // Arrange
        var repairProblem = new RepairProblem
        {
            Display = "Cupboards",
            Value = "cupboards",
        };
        var repair = new Repair
        {
            Problem = repairProblem
        };

        var expected = new FindRepairResponse
        {
            Problem = repairProblem
        };

        // Act
        var actual = systemUnderTest.Map(repair);

        // Assert
        actual.Problem.Should().BeEquivalentTo(expected.Problem);
    }

    [Fact]
    public void GivenRepairWithIssue_WhenMapping_ThenResponseHasAddress()
    {
        // Arrange
        var repairIssue = new RepairIssue
        {
            Display = "Door hanging off",
            Value = "dorrHangingOff",
        };
        var repair = new Repair
        {
            Issue = repairIssue
        };

        var expected = new FindRepairResponse
        {
            Issue = repairIssue
        };

        // Act
        var actual = systemUnderTest.Map(repair);

        // Assert
        actual.Issue.Should().BeEquivalentTo(expected.Issue);
    }

    [Fact]
    public void GivenRepairWithAppointmentTime_WhenMapping_ThenResponseHasAddress()
    {
        // Arrange
        var repairAppointment = new RepairAvailability
        {
            Display = "11th October 2022 between 8:00am and 12:00pm",
            StartDateTime = new DateTime(2022, 10, 11, 8, 0, 0),
            EndDateTime = new DateTime(2022, 10, 11, 12, 0, 0),
        };
        var repair = new Repair
        {
            Time = repairAppointment
        };

        var expected = new FindRepairResponse
        {
            AppointmentTime = repairAppointment
        };

        // Act
        var actual = systemUnderTest.Map(repair);

        // Assert
        actual.AppointmentTime.Should().BeEquivalentTo(expected.AppointmentTime);
    }
}
