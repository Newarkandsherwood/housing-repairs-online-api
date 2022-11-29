using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests;

public class RetrieveRepairsUseCaseTests
{
    private readonly RetrieveRepairsUseCase systemUnderTest;
    private readonly Mock<IRepairStorageGateway> repairStorageGatewayMock;
    private const string RepairId = "repairId";
    private const string Postcode = "postcode";

    public RetrieveRepairsUseCaseTests()
    {
        repairStorageGatewayMock = new Mock<IRepairStorageGateway>();
        systemUnderTest = new RetrieveRepairsUseCase(repairStorageGatewayMock.Object);
    }

    [Theory]
    [MemberData(nameof(InvalidRepairTypesParameter))]
    public async void GivenInvalidRepairTypesParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
        T exception, IEnumerable<string> repairTypes) where T : ArgumentException
    {
        // Arrange

        // Act
        var act = () => systemUnderTest.Execute(repairTypes, Postcode, RepairId);

        // Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    public static TheoryData<ArgumentException, IEnumerable<string>> InvalidRepairTypesParameter() =>
        new() {
            { new ArgumentNullException(), null },
            { new ArgumentException(), Enumerable.Empty<string>() },
        };

    [Theory]
    [MemberData(nameof(InvalidStringParameter))]
    public async void GivenInvalidPostcodeParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
        T exception, string postcode) where T : ArgumentException
    {
        // Arrange

        // Act
        var act = () => systemUnderTest.Execute(new[] { "repairType" }, postcode, RepairId);

        // Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidStringParameter))]
    public async void GivenInvalidRepairIdParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
        T exception, string repairId) where T : ArgumentException
    {
        // Arrange

        // Act
        var act = () => systemUnderTest.Execute(new[] { "repairType" }, Postcode, repairId);

        // Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    public static TheoryData<ArgumentException, string> InvalidStringParameter() =>
        new() {
            { new ArgumentNullException(), null },
            { new ArgumentException(), string.Empty },
            { new ArgumentException(), " " },
        };

    [Fact]
    public async void GivenMoreThanOneRepairFound_WhenExecuting_ThenNoRepairsAreReturned()
    {
        // Arrange
        var repairTypes = new[] { "repairType" };
        repairStorageGatewayMock.Setup(x => x.SearchByPostcodeAndId(repairTypes, Postcode, RepairId))
            .ReturnsAsync(new[] { new Repair(), new Repair() });

        // Act
        var actual = await systemUnderTest.Execute(repairTypes, Postcode, RepairId);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async void GivenRepairWithElapsedScheduledDateFound_WhenExecuting_ThenNoRepairsAreReturned()
    {
        // Arrange
        var repairTypes = new[] { "repairType" };
        repairStorageGatewayMock.Setup(x => x.SearchByPostcodeAndId(repairTypes, Postcode, RepairId))
            .ReturnsAsync(new[] { new Repair(), new Repair() });

        // Act
        var actual = await systemUnderTest.Execute(repairTypes, Postcode, RepairId);

        // Assert
        actual.Should().BeNull();
    }

}
