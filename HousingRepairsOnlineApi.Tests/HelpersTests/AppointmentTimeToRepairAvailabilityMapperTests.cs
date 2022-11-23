﻿using System;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class AppointmentTimeToRepairAvailabilityMapperTests
{
    private readonly AppointmentTimeToRepairAvailabilityMapper systemUnderTest = new();

    [Fact]
    public void GivenNullAppointmentTimeParameter_WhenMapping_ThenArgumentNullExceptionIsThrown()
    {
        // Arrange

        // Act
        var act = () => systemUnderTest.Map(null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GivenAppointmentTimeWithStartTime_WhenMapping_ThenReturnedRepairAvailabilityStartDateTimeIsSet()
    {
        // Arrange
        var startTime = new DateTime(2022, 11, 23, 8, 0, 0);
        var appointmentTime = new AppointmentTime { StartTime = startTime, };

        // Act
        var actual = systemUnderTest.Map(appointmentTime);

        // Assert
        actual.StartDateTime.Should().Be(startTime);
    }

    [Fact]
    public void GivenAppointmentTimeWithEndTime_WhenMapping_ThenReturnedRepairAvailabilityEndDateTimeIsSet()
    {
        // Arrange
        var endTime = new DateTime(2022, 11, 23, 12, 0, 0);
        var appointmentTime = new AppointmentTime { EndTime = endTime, };

        // Act
        var actual = systemUnderTest.Map(appointmentTime);

        // Assert
        actual.EndDateTime.Should().Be(endTime);
    }
}
