using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests;

public class RepairDescriptionRequestExtensionsTests
{
    [Fact]
    public void GivenALocationText_WhenAppendLocationDescription_ThenAppendedTextIsReturned()
    {
        var repairDescription = new RepairDescriptionRequest { Text = "Text", LocationText = "LocationText" };
        var result = repairDescription.CombinedDescriptionTexts();
        result.Should().Be("LocationText Text");
    }
    [Fact]
    public void GivenNoLocationText_WhenAppendLocationDescription_ThenTextIsReturned()
    {
        var repairDescription = new RepairDescriptionRequest { Text = "Text" };
        var result = repairDescription.CombinedDescriptionTexts();
        result.Should().Be("Text");
    }
}
