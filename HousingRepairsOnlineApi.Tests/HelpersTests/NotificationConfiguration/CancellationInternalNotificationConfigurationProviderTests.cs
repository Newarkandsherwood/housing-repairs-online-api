using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers.NotificationConfiguration;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests.NotificationConfiguration;

public class CancellationInternalNotificationConfigurationProviderTests
{
    private const string TemplateId = "templateID";
    private readonly CancellationInternalNotificationConfigurationProvider systemUnderTest = new(TemplateId);

    private Repair repair = new()
    {
        Id = "id",
        SOR = "SOR",
        Address = new RepairAddress
        {
            LocationId = "12345",
            Display = "display"
        },
        Description = new RepairDescription
        {
            Text = "description"
        },
    };

    [Fact]
    public void GivenTemplateId_WhenConstructing_TemplateIdIsSet()
    {
        // Arrange
        var expected = TemplateId;

        // Act

        // Assert
        var actual = systemUnderTest.TemplateId;
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidId_WhenGettingPersonalisationForTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Id = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidAddressLocationId_WhenGettingPersonalisationForTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Address.LocationId = invalidStr;
        // Act
        var act = () => systemUnderTest.GetPersonalisationForTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidDisplayAddress_WhenGettingPersonalisationForTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Address.Display = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidSor_WhenGettingPersonalisationForTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.SOR = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidDescriptionText_WhenGettingPersonalisationForTemplate_ThenExceptionIsThrown<T>(T exception, string invalidStr) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange
        repair.Description.Text = invalidStr;

        // Act
        var act = () => systemUnderTest.GetPersonalisationForTemplate(repair);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    public static IEnumerable<object[]> InvalidStringArgumentTestData()
    {
        yield return new object[] { new ArgumentNullException(), null };
        yield return new object[] { new ArgumentException(), "" };
        yield return new object[] { new ArgumentException(), " " };
    }

    [Fact]
    public void GivenARepairWithValidValues_WhenGettingPersonalisationForTemplate_ThenCorrectPersonalisationIsReturned()
    {
        //Arrange
        var personalisation = new Dictionary<string, dynamic>
        {
            {"repair_ref", repair.Id},
            {"uprn", repair.Address.LocationId},
            {"address", repair.Address.Display},
            {"sor", repair.SOR},
            {"repair_desc", repair.Description.Text},
        };

        //Act
        var actual = systemUnderTest.GetPersonalisationForTemplate(repair);

        //Assert
        actual.Should().BeEquivalentTo(personalisation);
    }
}
