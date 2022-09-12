using System.Linq;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class EarlyExitRepairTriageOptionMapperTests
    {
        private const string emergencyValue = "emergency";
        private const string notEligibleNonEmergencyValue = "notEligibleNonEmergency";
        private const string unableToBookValue = "unableToBook";
        private readonly EarlyExitRepairTriageOptionMapper systemUnderTest = new();

        [Fact]
        public void GivenOptionsWithoutValuesNeedingMapping_WhenExecuting_ThenReturnedValueDoesNotContainMappedValues()
        {
            // Arrange
            var expected = new RepairTriageOption { Value = "doorHangingOff" };
            var input = new[] { expected };

            // Act
            var repairTriageOptions = systemUnderTest.MapRepairTriageOption(input, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue);
            var actual = repairTriageOptions.FirstOrDefault();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(EarlyExitValues.EmergencyExitValue, emergencyValue)]
        [InlineData(EarlyExitValues.UnableToBook, unableToBookValue)]
        [InlineData(EarlyExitValues.NotEligibleNonEmergency, notEligibleNonEmergencyValue)]
        public void GivenValuesNeedingMapping_WhenExecuting_ThenReturnedValueContainsMappedValues(string triageOptionValue, string triageOptionsMappedValue)
        {
            // Arrange
            var input = new[] { new RepairTriageOption { Value = triageOptionValue } };
            var expected = new RepairTriageOption { Value = triageOptionsMappedValue };

            // Act
            var repairTriageOptions = systemUnderTest.MapRepairTriageOption(input, emergencyValue, notEligibleNonEmergencyValue, unableToBookValue);
            var actual = repairTriageOptions.FirstOrDefault();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
