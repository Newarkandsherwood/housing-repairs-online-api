using System;
using FluentAssertions;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class EnvironmentVariableRepairTypeSorConfigurationProviderTests
{
    [Theory]
    [MemberData(nameof(Helpers.RepairTypeTestData.InvalidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
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

    [Theory]

    [MemberData(nameof(Helpers.RepairTypeTestData.ValidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
    public void GivenValidRepairTypeParameter_WhenConstructing_ThenExceptionIsNotThrown(string repairTypeParameter)
    {
        // Arrange

        // Act
        var act = () => new EnvironmentVariableRepairTypeSorConfigurationProvider(repairTypeParameter);

        // Assert
        act.Should().NotThrow<ArgumentException>();
    }
}
