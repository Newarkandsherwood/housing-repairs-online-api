using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class LargestAppointmentSlotResolverTests
{
    [Fact]
    public void GivenNoAppointmentSlots_WhenResolving_ThenNoAppointmentSlotsAreReturned()
    {
        // Arrange
        var systemUnderTest = new LargestAppointmentSlotFilter();

        // Act
        var actual = systemUnderTest.Filter(Enumerable.Empty<AppointmentSlotTimeSpan>());

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public void GivenSingleAppointmentSlot_WhenResolving_ThenSingleAppointmentSlotIsReturned()
    {
        // Arrange
        var appointmentSlots = new[]
        {
            new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(2) }
        };
        var systemUnderTest = new LargestAppointmentSlotFilter();

        // Act
        var actual = systemUnderTest.Filter(appointmentSlots);

        // Assert
        actual.Should().BeEquivalentTo(appointmentSlots);
    }

    [Theory]
    [MemberData(nameof(MultipleVaryingSizeAppointmentSlots))]
    public void GivenMultipleVaryingSizeAppointmentSlots_WhenResolving_ThenSingleLargestAppointmentSlotIsReturned(IEnumerable<AppointmentSlotTimeSpan> appointmentSlots, AppointmentSlotTimeSpan expected)
    {
        // Arrange
        var systemUnderTest = new LargestAppointmentSlotFilter();

        // Act
        var actual = systemUnderTest.Filter(appointmentSlots);

        // Assert
        actual.Should().BeEquivalentTo(new[]
        {
            expected
        });
    }

    public static TheoryData<IEnumerable<AppointmentSlotTimeSpan>, AppointmentSlotTimeSpan>
        MultipleVaryingSizeAppointmentSlots() =>
        new()
        {
            {
                new[]
                {
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(2) },
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(5) },
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(10) },
                },
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(10) }
            },
            {
                new[]
                {
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(5) },
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(10) },
                    new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(2) },
                },
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(1), EndTime = new TimeSpan(10) }
            },
            {
                new[]
                {
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0)
                    },
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0)
                    },
                },
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
            },
            {
                new[]
                {
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0)
                    },
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0)
                    },
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(14, 30, 0)
                    },
                },
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
            },
            {
                new[]
                {
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0)
                    },
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(18, 0, 0)
                    },
                    new AppointmentSlotTimeSpan
                    {
                        StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0)
                    },
                },
                new AppointmentSlotTimeSpan { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
            },
        };
}
