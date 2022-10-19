using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class SorEngineResolverTests
{
    private readonly SorEngineResolver systemUnderTest;
    private readonly Dictionary<string, ISoREngine> sorEngines = new();

    public SorEngineResolverTests()
    {
        systemUnderTest = new SorEngineResolver(sorEngines);
    }

    [Fact]
    public void GivenNullSorEnginesParameter_WhenConstructing_ThenExceptionIsThrown()
    {
        // Arrange

        // Act
        var act = () => new SorEngineResolver(null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
    public void GivenInvalidRepairTypeParameter_WhenResolving_ThenExceptionIsThrown<T>(T exception,
        string repairTypeParameter) where T : Exception
    {
        // Arrange

        // Act
        Action act = () => _ = systemUnderTest.Resolve(repairTypeParameter);

        // Assert
        act.Should().Throw<T>();
    }

    public static TheoryData<Exception, string> InvalidArgumentTestData()
    {
        return new()
        {
            { new ArgumentNullException(), null },
            { new ArgumentException(), string.Empty },
            { new ArgumentException(), " " },
            { new ArgumentException(), "non-repair-type-value" },
        };
    }

    [Theory]
    [MemberData(nameof(ValidRepairTypeArgumentTestData))]
    public void GivenValidRepairTypeParameter_WhenResolving_ThenExceptionIsNotThrown(string repairTypeParameter)
    {
        // Arrange

        // Act
        Action act = () => _ = systemUnderTest.Resolve(repairTypeParameter);

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

    [Fact]
    public void GivenSorEngineForRepairType_WhenResolving_ThenSorEngineIsReturned()
    {
        // Arrange
        var repairType = RepairType.Tenant;
        ISoREngine sorEngine = default;
        sorEngines[repairType] = sorEngine;

        // Act
        var actual = systemUnderTest.Resolve(repairType);

        // Assert
        actual.Should().BeSameAs(sorEngine);
    }

    [Fact]
    public void GivenLackOfSorEngineForRepairType_WhenResolving_ThenExceptionIsThrown()
    {
        // Arrange
        var repairType = RepairType.Tenant;

        // Act
        var act = () => systemUnderTest.Resolve(repairType);

        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}
