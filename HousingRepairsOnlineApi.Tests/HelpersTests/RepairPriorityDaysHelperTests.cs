using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class RepairPriorityDaysHelperTests
    {
        private RepairPriorityDaysHelper systemUnderTest;

        private readonly Repair repair = new();

        public RepairPriorityDaysHelperTests()
        {
            IEnumerable<RepairPriorityDays> RepairPriorityDays = new RepairPriorityDays[]
            {
                new() { NumberOfDays = 3, Priority = "2" },
                new() { NumberOfDays = 30, Priority = "3" }
            };
            systemUnderTest = new RepairPriorityDaysHelper(RepairPriorityDays);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayPrioritys_WhenCallingGetDaysForRepair_ReturnsCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            repair.Priority = "2";

            // Act
            var result = systemUnderTest.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayPrioritys_WhenCallingGetDaysForRepairForSecondPriority_ReturnsCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            repair.Priority = "3";

            // Act
            var result = systemUnderTest.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayPrioritys_WhenCallingGetDaysForRepairForSecondPriorityWithManyResults_ReturnsFirstCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            var repairPriorityDaysHelper = new RepairPriorityDaysHelper(new RepairPriorityDays[]
            {
                new() { NumberOfDays = 3, Priority = "2" }, new() { NumberOfDays = 30, Priority = "3" }, new() { NumberOfDays = 3, Priority = "7" }
            });
            repair.Priority = "2";

            // Act
            var result = repairPriorityDaysHelper.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayPrioritys_WhenCallingGetDaysForRepairForMultipleWithSamePriority_ReturnsFirstCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            var repairPriorityDaysHelper = new RepairPriorityDaysHelper(new RepairPriorityDays[]
            {
                new() { NumberOfDays = 3, Priority = "2" }, new() { NumberOfDays = 30, Priority = "2" }, new() { NumberOfDays = 130, Priority = "2" }
            });
            repair.Priority = "2";

            // Act
            var result = repairPriorityDaysHelper.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(3, result);
        }
        [Fact]
#pragma warning disable CA1707
        public void GivenNoRepairDayPriorities_WhenCallingGetDaysForRepair_Returns0()
#pragma warning restore CA1707
        {
            // Arrange
            RepairPriorityDaysHelper repairPriorityDaysHelper = new RepairPriorityDaysHelper(Array.Empty<RepairPriorityDays>());
            repair.Priority = "2";

            // Act
            var result = repairPriorityDaysHelper.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenNoMatchingRepairDayPriorities_WhenCallingGetDaysForRepair_Returns0()
#pragma warning restore CA1707
        {
            // Arrange
            var repairPriorityDaysHelper = new RepairPriorityDaysHelper(new RepairPriorityDays[]
            {
                new() { NumberOfDays = 3, Priority = "2" }, new() { NumberOfDays = 30, Priority = "3" }, new() { NumberOfDays = 130, Priority = "4" }
            });
            repair.Priority = "5";

            // Act
            var result = repairPriorityDaysHelper.GetDaysForRepair(repair);

            // Assert
            Assert.Equal(0, result);
        }
        [Fact]
#pragma warning disable CA1707
        public void GivenNullRepair_WhenCallingGetDaysForRepair_throwsException()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Action act = () => _ = systemUnderTest.GetDaysForRepair(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
