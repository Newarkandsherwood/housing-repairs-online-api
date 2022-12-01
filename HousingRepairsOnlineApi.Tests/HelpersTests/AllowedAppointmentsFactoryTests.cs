using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class AllowedAppointmentsFactoryTests
{
    private AllowedAppointmentsFactory systemUnderTest;
    private readonly AppointmentSlotTimeSpan[] allAllowedAppointmentSlots;
    private readonly Dictionary<string, IAppointmentSlotsFilter> appointmentSlotsFilterByRepairType;

    private readonly AppointmentSlotTimeSpan firstAppointmentSlotTimeSpan =
        new() { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0) };

    private readonly AppointmentSlotTimeSpan secondAppointmentSlotTimeSpan =
        new() { StartTime = new TimeSpan(12, 0, 0), EndTime = new TimeSpan(16, 0, 0) };

    public AllowedAppointmentsFactoryTests()
    {
        allAllowedAppointmentSlots = new[]
        {
            firstAppointmentSlotTimeSpan,
            secondAppointmentSlotTimeSpan,
        };

        appointmentSlotsFilterByRepairType = new Dictionary<string, IAppointmentSlotsFilter>();

        systemUnderTest =
            new AllowedAppointmentsFactory(allAllowedAppointmentSlots, appointmentSlotsFilterByRepairType);
    }

    [Fact]
    public void GivenNullAllAllowedAppointmentSlots_WhenConstructing_ThenExceptionIsThrown()
    {
        // Arrange

        //Act
        Action act = () =>
            new AllowedAppointmentsFactory(null, ImmutableDictionary<string, IAppointmentSlotsFilter>.Empty);

        //Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void GivenNullAppointmentSlotsFilterByRepairType_WhenConstructing_ThenExceptionIsThrown()
    {
        // Arrange

        //Act
        Action act = () => new AllowedAppointmentsFactory(Array.Empty<AppointmentSlotTimeSpan>(), null);

        //Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void GivenEmptyAllAllowedAppointmentSlots_WhenCreatingAllowedAppointmentSlots_ThenNoAppointmentSlotsAreReturned()
    {
        // Arrange
        var expected = Array.Empty<AppointmentSlotTimeSpan>();
        systemUnderTest = new AllowedAppointmentsFactory(Array.Empty<AppointmentSlotTimeSpan>(), ImmutableDictionary<string, IAppointmentSlotsFilter>.Empty);

        //Act
        var actual = systemUnderTest.CreateAllowedAppointments("repairType");

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GivenEmptyAppointmentSlotsFilterByRepairType_WhenCreatingAllowedAppointmentSlots_ThenAllAppointmentSlotsAreReturned()
    {
        // Arrange
        var expected = new[]
        {
            firstAppointmentSlotTimeSpan,
            secondAppointmentSlotTimeSpan,
        };

        //Act
        var actual = systemUnderTest.CreateAllowedAppointments("repairType");

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidCreateAllowedAppointmentsRepairTypeParameter))]
    public void GivenInvalidRepairTypeParameter_WhenCreatingAllowedAppointmentSlots_ThenExceptionIsThrown<T>(
        T exception, string repairType)
        where T : Exception
    {
        // Arrange

        //Act
        Action act = () => systemUnderTest.CreateAllowedAppointments(repairType);

        //Assert
        act.Should().ThrowExactly<T>();
    }

    [Fact]
    public void GivenRepairTypeParameterWhichIsNotPresent_WhenCreatingAllowedAppointmentSlots_ThenNoExceptionIsThrown()
    {
        // Arrange

        //Act
        Action act = () => systemUnderTest.CreateAllowedAppointments("missing");

        //Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GivenRepairTypeParameterWhichIsNotPresent_WhenCreatingAllowedAppointmentSlots_ThenAllAllowedAppointmentsAreReturned()
    {
        // Arrange

        //Act
        var actual = systemUnderTest.CreateAllowedAppointments("missing");

        //Assert
        actual.Should().BeEquivalentTo(allAllowedAppointmentSlots);
    }

    [Fact]
    public void GivenRepairTypeParameter_WhenCreatingAllowedAppointmentSlots_ThenAllowedAppointmentsForRepairTypeAreReturned()
    {
        // Arrange
        var expected = new[]
        {
            firstAppointmentSlotTimeSpan,
        };

        var appointmentSlotsFilterMock = new Mock<IAppointmentSlotsFilter>();
        appointmentSlotsFilterMock
            .Setup(x => x.Filter(It.IsAny<IEnumerable<AppointmentSlotTimeSpan>>()))
            .Returns(expected);
        var repairType = "firstOneOnly";
        appointmentSlotsFilterByRepairType.Add(repairType, appointmentSlotsFilterMock.Object);

        //Act
        var actual = systemUnderTest.CreateAllowedAppointments(repairType);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    public static TheoryData<Exception, string> InvalidCreateAllowedAppointmentsRepairTypeParameter()
    {
        return new()
        {
            { new ArgumentNullException(), null },
            { new ArgumentException(), "" },
            { new ArgumentException(), "    " }
        };
    }
}
