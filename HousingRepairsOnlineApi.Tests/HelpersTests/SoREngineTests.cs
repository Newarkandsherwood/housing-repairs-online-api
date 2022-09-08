using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class SoREngineTests
    {
        private readonly SoREngine systemUnderTest;

        public SoREngineTests()
        {
            var mapping = new Dictionary<string, IDictionary<string, dynamic>>()

            {
                {
                    "kitchen", new Dictionary<string, dynamic>
                    {
                        {
                            "cupboards", new Dictionary<string, string>
                            {
                                { "doorHangingOff", "N373049" },
                                { "doorMissing", "N373049" },
                            }
                        },
                        {
                            "worktop", "N372005"
                        },
                    }
                },
                {
                    "bathroom", new Dictionary<string, dynamic>
                    {
                        {
                            "bath", new Dictionary<string, string>
                            {
                                { "bathTaps", "N631301" }
                            }
                        }
                    }
                }
            };

            var journeyTriageOptions = Array.Empty<RepairTriageOption>();
            systemUnderTest = new SoREngine(mapping, journeyTriageOptions);
        }

        [Theory]
        [InlineData("kitchen", "cupboards", "doorHangingOff", "N373049")]
        [InlineData("kitchen", "cupboards", "doorMissing", "N373049")]
        [InlineData("kitchen", "worktop", null, "N372005")]
        [InlineData("bathroom", "bath", "bathTaps", "N631301")]
        public void GivenLocationProblemIssue_WhenCallingMapSorCode_ThenExpectedSorIsReturned(string location, string problem, string issue, string expectedSor)
        {
            // Arrange

            // Act
            var actual = systemUnderTest.MapSorCode(location, problem, issue);

            // Assert
            Assert.Equal(expectedSor, actual);
        }

        [Fact]
        public void GivenRepairTriageOptions_WhenRepairTriageOptionsCalled_RepairTriageOptionsReturned()
        {
            // Arrange
            var journeyTriageOptions = Array.Empty<RepairTriageOption>();

            // Act
            var actual = systemUnderTest.RepairTriageOptions();

            // Assert
            actual.Should().BeEquivalentTo(journeyTriageOptions);
        }
    }
}
