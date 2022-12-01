using System;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class RepairDescriptionRequestToRepairDescriptionMapperTests
{

    private readonly RepairDescriptionRequestToRepairDescriptionMapper systemUnderTest;

    public RepairDescriptionRequestToRepairDescriptionMapperTests()
    {
        systemUnderTest = new RepairDescriptionRequestToRepairDescriptionMapper();
    }

    [Fact]
    public void GivenACommunalRepairRequestWhenMapIsCalledThenRepairDescriptionWithNewLineIsReturned()
    {
        const string repairType = "COMMUNAL";

        var repairRequest = new RepairDescriptionRequest { Text = "Text", LocationDescription = "Location" };

        var repair = systemUnderTest.Map(repairRequest, repairType);

        Assert.Equal("Location" + Environment.NewLine + "Text", repair.Text);

    }

    [Fact]
    public void GivenATenantRepairRequestWhenMapIsCalledThenRepairDescriptionIsReturnedWithoutSpace()
    {
        const string repairType = "TENANT";

        var repairRequest = new RepairDescriptionRequest { Text = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis." };

        var repair = systemUnderTest.Map(repairRequest, repairType);

        Assert.Equal("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis.", repair.Text);
    }
}
