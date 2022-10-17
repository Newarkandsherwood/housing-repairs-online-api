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
                            "cupboards", new Dictionary<string, RepairTriageDetails>
                            {
                                { "doorHangingOff", new RepairTriageDetails { ScheduleOfRateCode = "N373049", Priority = "1" } },
                                { "doorMissing", new RepairTriageDetails { ScheduleOfRateCode = "N373049", Priority = "1" } },
                            }
                        },
                        {
                            "worktop", new RepairTriageDetails { ScheduleOfRateCode = "N372005", Priority = "2" }
                        },
                    }
                },
                {
                    "bathroom", new Dictionary<string, dynamic>
                    {
                        {
                            "bath", new Dictionary<string, RepairTriageDetails>
                            {
                                { "bathTaps", new RepairTriageDetails { ScheduleOfRateCode = "N631301", Priority = "3" } }
                            }
                        }
                    }
                }
            };

            var journeyTriageOptions = Array.Empty<RepairTriageOption>();
            systemUnderTest = new SoREngine(mapping, journeyTriageOptions);
        }

        [Theory]
        [InlineData("kitchen", "cupboards", "doorHangingOff", "N373049", "1")]
        [InlineData("kitchen", "cupboards", "doorMissing", "N373049", "1")]
        [InlineData("kitchen", "worktop", null, "N372005", "2")]
        [InlineData("bathroom", "bath", "bathTaps", "N631301", "3")]
        public void GivenLocationProblemIssue_WhenCallingMapSorCode_ThenExpectedSorIsReturned(string location, string problem, string issue, string expectedSor, string expectedPriority)
        {
            // Arrange
            var expected = new RepairTriageDetails { ScheduleOfRateCode = expectedSor, Priority = expectedPriority };

            // Act
            var actual = systemUnderTest.MapSorCode(location, problem, issue);

            // Assert
            actual.Should().BeEquivalentTo(expected);
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
