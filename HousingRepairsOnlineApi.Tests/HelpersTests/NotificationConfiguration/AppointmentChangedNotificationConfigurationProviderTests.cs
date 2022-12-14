using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests.NotificationConfiguration;

public class AppointmentChangedNotificationConfigurationProviderTests
{
    private const string SmsTemplateId = "templateID";
    private const string EmailTemplateId = "templateID";
    private readonly AppointmentChangedNotificationConfigurationProvider systemUnderTest = new(SmsTemplateId, EmailTemplateId);

    private readonly Repair repair = new()
    {
        Id = "id",
        Time = new RepairAvailability
        {
            Display = "A date and time slot",
        }
    };

    [Fact]
    public void GivenSmsTemplateId_WhenConstructing_TemplateIdIsSet()
    {
        // Arrange
        var expected = SmsTemplateId;

        // Act

        // Assert
        var actual = systemUnderTest.SmsTemplateId;
        actual.Should().Be(expected);
    }

    [Fact]
    public void GivenEmailTemplateId_WhenConstructing_TemplateIdIsSet()
    {
        // Arrange
        var expected = EmailTemplateId;

        // Act

        // Assert
        var actual = systemUnderTest.EmailTemplateId;
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidId_WhenGettingPersonalisationForSmsTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Id = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForSmsTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Fact]
#pragma warning disable xUnit1026
    public void GivenANullTime_WhenGettingPersonalisationForSmsTemplate_ThenExceptionIsThrown()
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Time = null;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForSmsTemplate(repair);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidTimeDisplay_WhenGettingPersonalisationForSmsTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Time.Display = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForSmsTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Fact]
    public void GivenARepairWithValidValues_WhenGettingPersonalisationForSmsTemplate_ThenCorrectPersonalisationIsReturned()
    {
        //Arrange
        var personalisation = new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"appointment_time", repair.Time.Display},
        };

        //Act
        var actual = systemUnderTest.GetPersonalisationForSmsTemplate(repair);

        //Assert
        actual.Should().BeEquivalentTo(personalisation);
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidId_WhenGettingPersonalisationForEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Id = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Fact]
#pragma warning disable xUnit1026
    public void GivenANullTime_WhenGettingPersonalisationForEmailTemplate_ThenExceptionIsThrown()
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Time = null;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidTimeDisplay_WhenGettingPersonalisationForEmailTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Time.Display = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForEmailTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Fact]
    public void GivenARepairWithValidValues_WhenGettingPersonalisationForEmailTemplate_ThenCorrectPersonalisationIsReturned()
    {
        //Arrange
        var personalisation = new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"appointment_time", repair.Time.Display},
        };

        //Act
        var actual = systemUnderTest.GetPersonalisationForEmailTemplate(repair);

        //Assert
        actual.Should().BeEquivalentTo(personalisation);
    }

    public static IEnumerable<object[]> InvalidStringArgumentTestData()
    {
        yield return new object[] { new ArgumentNullException(), null };
        yield return new object[] { new ArgumentException(), "" };
        yield return new object[] { new ArgumentException(), " " };
    }
}
