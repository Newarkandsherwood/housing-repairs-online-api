using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.Helpers;

public class RepairRequestToRepairMapperTests
{
    private readonly Mock<ISoREngine> mockSorEngine;
    private readonly Mock<ISorEngineResolver> mockSorEngineResolver;
    private readonly RepairRequestToRepairMapper systemUnderTest;

    private readonly Mock<IRepairDescriptionRequestToRepairDescriptionMapper>
        mockRepairDescriptionRequestToRepairDescriptionMapper;


    public RepairRequestToRepairMapperTests()
    {
        mockSorEngine = new Mock<ISoREngine>();
        mockSorEngineResolver = new Mock<ISorEngineResolver>();
        mockSorEngineResolver.Setup(x => x.Resolve(It.IsAny<string>())).Returns(mockSorEngine.Object);
        mockRepairDescriptionRequestToRepairDescriptionMapper =
            new Mock<IRepairDescriptionRequestToRepairDescriptionMapper>();
        systemUnderTest = new RepairRequestToRepairMapper(mockSorEngineResolver.Object, mockRepairDescriptionRequestToRepairDescriptionMapper.Object);
    }

    [Fact]
    public void GivenACommunalRepairRequestWhenMapIsCalledThenRepairIsReturned()
    {
        const string Location = "kitchen";
        const string Problem = "cupboards";
        const string Issue = "doorHangingOff";
        const string RepairCode = "N373049";
        const string Priority = "priority";
        const string repairType = "Communal";

        var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = RepairCode, Priority = Priority };

        var repairRequest = new RepairRequest
        {
            Location = new RepairLocation { Value = Location },
            Problem = new RepairProblem { Value = Problem },
            Issue = new RepairIssue { Value = Issue },
            Description = new RepairDescriptionRequest { Text = "Text", LocationText = "LocationText" }
        };

        mockSorEngine.Setup(x => x.MapToRepairTriageDetails(Location, Problem, Issue))
            .Returns(repairTriageDetails);
        mockRepairDescriptionRequestToRepairDescriptionMapper.Setup(x =>
            x.Map(It.IsAny<RepairDescriptionRequest>(), It.IsAny<string>())).Returns(new RepairDescription() { Text = "LocationText Text" });

        var repair = systemUnderTest.Map(repairRequest, repairType);

        mockSorEngine.Verify(x => x.MapToRepairTriageDetails(Location, Problem, Issue), Times.Once);

        Assert.Equal("LocationText Text", repair.Description.Text);
        Assert.Equal(Location, repair.Location.Value);
        Assert.Equal(Problem, repair.Problem.Value);
        Assert.Equal(Issue, repair.Issue.Value);

    }

    [Fact]
    public void GivenATenantRepairRequestWhenMapIsCalledThenRepairIsReturned()
    {
        const string Location = "kitchen";
        const string Problem = "cupboards";
        const string Issue = "doorHangingOff";
        const string RepairCode = "N373049";
        const string Priority = "priority";
        const string repairType = "Tenant";

        var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = RepairCode, Priority = Priority };

        var repairRequest = new RepairRequest
        {
            Location = new RepairLocation { Value = Location },
            Problem = new RepairProblem { Value = Problem },
            Issue = new RepairIssue { Value = Issue },
            Description = new RepairDescriptionRequest { Text = "Text" }
        };

        mockSorEngine.Setup(x => x.MapToRepairTriageDetails(Location, Problem, Issue))
            .Returns(repairTriageDetails);

        systemUnderTest.Map(repairRequest, repairType);

        mockRepairDescriptionRequestToRepairDescriptionMapper.Verify(x => x.Map(repairRequest.Description, repairType), Times.Once);
        mockSorEngine.Verify(x => x.MapToRepairTriageDetails(Location, Problem, Issue), Times.Once);

    }
}
