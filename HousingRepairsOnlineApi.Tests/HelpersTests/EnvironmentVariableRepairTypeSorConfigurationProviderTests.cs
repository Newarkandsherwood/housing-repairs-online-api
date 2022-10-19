using System;
using FluentAssertions;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class EnvironmentVariableRepairTypeSorConfigurationProviderTests
{
    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public void GivenAnInvalidRepairType_WhenConstructing_ThenExceptionIsThrown<T>(T exception, string repairTypeParameter) where T : Exception
#pragma warning restore xUnit1026
    {
        // Arrange

        // Act
        var act = () => new EnvironmentVariableRepairTypeSorConfigurationProvider(repairTypeParameter);

        // Assert
        act.Should().ThrowExactly<T>();
    }

    public static TheoryData<Exception, string> InvalidArgumentTestData() =>
        new()
        {
            { new ArgumentNullException(), null },
            { new ArgumentException(), "" },
            { new ArgumentException(), " " },
            { new ArgumentException(), "non-repair-type-value" },
        };

    [Theory]

    [MemberData(nameof(ValidRepairTypeArgumentTestData))]
    public void GivenValidRepairTypeParameter_WhenConstructing_ThenExceptionIsNotThrown(string repairTypeParameter)
    {
        // Arrange

        // Act
        var act = () => new EnvironmentVariableRepairTypeSorConfigurationProvider(repairTypeParameter);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }

    public static TheoryData<string> ValidRepairTypeArgumentTestData()
    {
        var result = new TheoryData<string>();
        foreach (var repairType in RepairType.All)
        {
            result.Add(repairType);
        }
        return result;
    }
}
