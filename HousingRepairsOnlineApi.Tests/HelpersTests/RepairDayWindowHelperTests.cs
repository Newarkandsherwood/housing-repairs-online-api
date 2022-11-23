using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class RepairDayWindowHelperTests
    {
        private RepairDayWindowHelper systemUnderTest;

        private readonly Repair _repair = new();

        public RepairDayWindowHelperTests()
        {
            IEnumerable<RepairDayWindow> repairDayWindows = new List<RepairDayWindow>()
            {
                new() { NumberOfDays = 3, Priority = "2" },
                new() { NumberOfDays = 30, Priority = "3" }
            };
            systemUnderTest = new RepairDayWindowHelper(repairDayWindows);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayWindows_WhenCallingGetDaysForRepair_ReturnsCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            _repair.Priority = "2";

            // Act
            var result = systemUnderTest.GetDaysForRepair(_repair);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayWindows_WhenCallingGetDaysForRepairForSecondWindow_ReturnsCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            _repair.Priority = "3";

            // Act
            var result = systemUnderTest.GetDaysForRepair(_repair);

            // Assert
            Assert.Equal(30, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenRepairDayWindows_WhenCallingGetDaysForRepairForSecondWindowWithManyResults_ReturnsFirstCorrectDays()
#pragma warning restore CA1707
        {
            // Arrange
            RepairDayWindowHelper repairDayWindowHelper = new RepairDayWindowHelper(new List<RepairDayWindow>()
            {
                new() { NumberOfDays = 3, Priority = "2" }, new() { NumberOfDays = 30, Priority = "3" }, new() { NumberOfDays = 3, Priority = "7" }
            });
            _repair.Priority = "2";

            // Act
            var result = repairDayWindowHelper.GetDaysForRepair(_repair);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenNoRepairDayWindows_WhenCallingGetDaysForRepairForSecondWindowWithManyResults_Returns0()
#pragma warning restore CA1707
        {
            // Arrange
            RepairDayWindowHelper repairDayWindowHelper = new RepairDayWindowHelper(new List<RepairDayWindow>());
            _repair.Priority = "2";

            // Act
            var result = repairDayWindowHelper.GetDaysForRepair(_repair);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenNullRepair_WhenCallingGetDaysForRepair_throwsException()
#pragma warning restore CA1707
        {
            // Arrange
            RepairDayWindowHelper repairDayWindowHelper = new RepairDayWindowHelper(new List<RepairDayWindow>()
            {
                new() { NumberOfDays = 3, Priority = "2" }, new() { NumberOfDays = 30, Priority = "3" }, new() { NumberOfDays = 3, Priority = "7" }
            });
            _repair.Priority = "2";

            // Act
            Action act = () => _= systemUnderTest.GetDaysForRepair(null);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
