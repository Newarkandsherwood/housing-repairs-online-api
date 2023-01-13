using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests;

public class CreateWorkOrderUseCaseTests
{
    private readonly Mock<IWorkOrderGateway> workOrderGatewayMock;
    private readonly Mock<ISorEngineResolver> sorEngineResolverMock;
    private readonly CreateWorkOrderUseCase systemUnderTest;
    private const string RepairType = HousingRepairsOnlineApi.Helpers.RepairType.Tenant;

    public CreateWorkOrderUseCaseTests()
    {
        workOrderGatewayMock = new Mock<IWorkOrderGateway>();
        sorEngineResolverMock = new Mock<ISorEngineResolver>();
        systemUnderTest = new CreateWorkOrderUseCase(workOrderGatewayMock.Object, sorEngineResolverMock.Object);
    }

    public static IEnumerable<object[]> InvalidArgumentTestData()
    {
        yield return new object[] { new ArgumentNullException(), null };
        yield return new object[] { new ArgumentException(), "" };
        yield return new object[] { new ArgumentException(), " " };
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidLocationId_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string locationId)
        where T : Exception
#pragma warning restore xUnit1026
    {
        //Act
        var repairRequest = new RepairRequest
        {
            Address = new RepairAddress
            {
                LocationId = locationId
            }
        };
        repairRequest.Address.LocationId = locationId;
        Func<Task> act = async () => await systemUnderTest.Execute(RepairType, repairRequest);

        //Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidRepairLocation_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string repairLocation)
        where T : Exception
#pragma warning restore xUnit1026
    {
        //Act
        var repairRequest = new RepairRequest
        {
            Address = new RepairAddress
            {
                LocationId = "locationId"
            },
            Location = new RepairLocation()
            {
                Value = repairLocation
            }
        };
        Func<Task> act = async () => await systemUnderTest.Execute(RepairType, repairRequest);

        //Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidRepairProblem_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string repairProblem)
        where T : Exception
#pragma warning restore xUnit1026
    {
        //Act
        var repairRequest = new RepairRequest
        {
            Address = new RepairAddress
            {
                LocationId = "locationId"
            },
            Location = new RepairLocation()
            {
                Value = "repairLocation"
            },
            Problem = new RepairProblem()
            {
                Value = repairProblem
            }
        };
        Func<Task> act = async () => await systemUnderTest.Execute(RepairType, repairRequest);

        //Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Theory]
    [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidDescription_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string description)
        where T : Exception
#pragma warning restore xUnit1026
    {
        //Act
        var repairRequest = new RepairRequest
        {
            Address = new RepairAddress
            {
                LocationId = "locationId"
            },
            Location = new RepairLocation()
            {
                Value = "repairLocation"
            },
            Problem = new RepairProblem()
            {
                Value = "repairProblem"
            },
            Description = new RepairDescriptionRequest()
            {
                Text = description
            }
        };
        Func<Task> act = async () => await systemUnderTest.Execute(RepairType, repairRequest);

        //Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Theory]
    [MemberData(nameof(Helpers.RepairTypeTestData.InvalidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
#pragma warning disable xUnit1026
    public async void GivenAnInvalidRepairType_WhenExecuting_ThenExceptionIsThrown<T>(T exception, string repairType)
        where T : Exception
#pragma warning restore xUnit1026
    {
        //Act
        var repairRequest = new RepairRequest();

        Func<Task> act = async () => await systemUnderTest.Execute(repairType, repairRequest);

        //Assert
        await act.Should().ThrowExactlyAsync<T>();
    }

    [Fact]
    public async void GivenValidArguments_WhenExecuting_ThenWorkOrderGatewayIsCalledWithCorrectParameters()
    {
        // Arrange
        var locationId = "locationId";
        var repairlocation = "repairLocation";
        var repairproblem = "repairProblem";
        var description = "description";

        var repairRequest = new RepairRequest
        {
            Address= new RepairAddress
            {
                LocationId = locationId
            },
            Location = new RepairLocation()
            {
                Value = repairlocation
            },
            Problem = new RepairProblem()
            {
                Value = repairproblem
            },
            Description = new RepairDescriptionRequest()
            {
                Text = description
            }
        };

        var scheduleOfRateCode = "sorCode";

        workOrderGatewayMock.Setup(x => x.CreateWorkOrder(locationId, scheduleOfRateCode, description));
        var sorEngineMock = new Mock<ISoREngine>();
        sorEngineMock.Setup(x => x.MapToRepairTriageDetails(repairlocation, repairproblem, It.IsAny<string>()))
            .Returns(new RepairTriageDetails { ScheduleOfRateCode = scheduleOfRateCode });
        sorEngineResolverMock.Setup(x => x.Resolve(It.IsAny<string>())).Returns(sorEngineMock.Object);

        // Act
        await systemUnderTest.Execute(RepairType, repairRequest);

        // Assert
        workOrderGatewayMock.Verify(
            x => x.CreateWorkOrder(locationId, scheduleOfRateCode, It.Is<string>(x => x.Contains(description))),
            Times.Once);
    }
}
