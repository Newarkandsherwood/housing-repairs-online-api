using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class RepairDescriptionExtensionsTests
{
    [Fact]
    public void GivenALocationText_WhenAppendLocationDescription_ThenAppendedTextIsReturned()
    {
        var repairDescription = new RepairDescription { Text = "Text", LocationText = "LocationText" };
        var result = repairDescription.AppendLocationDescription();
        result.Should().Be("LocationText Text");
    }
    [Fact]
    public void GivenNoLocationText_WhenAppendLocationDescription_ThenTextIsReturned()
    {
        var repairDescription = new RepairDescription { Text = "Text"};
        var result = repairDescription.AppendLocationDescription();
        result.Should().Be("Text");
    }
}
