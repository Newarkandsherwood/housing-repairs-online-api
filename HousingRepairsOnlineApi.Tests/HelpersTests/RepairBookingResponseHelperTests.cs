using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class RepairBookingResponseHelperTests
    {
        private readonly RepairToRepairBookingResponseMapper systemUnderTest;
        private readonly Repair repair = new();
        private readonly int daysForRepair = 30;

        public RepairBookingResponseHelperTests()
        {
            var RepairPriorityDaysHelperMock = new Mock<IRepairPriorityDaysHelper>();
            RepairPriorityDaysHelperMock.Setup(_ => _.GetDaysForRepair(It.IsAny<Repair>())).Returns(daysForRepair);
            repair.Id = "repairId";
            systemUnderTest = new RepairToRepairBookingResponseMapper(RepairPriorityDaysHelperMock.Object);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenATenantRepair_OnGetRepairBookingResponse_RepairBookingResponseIsReturnedWithRepairId()
#pragma warning restore CA1707
        {
            // Arrange
            repair.RepairType = RepairType.Tenant;

            // Act
            var result = systemUnderTest.MapRepairBookingResponse(repair, false);

            // Assert
            Assert.IsType<RepairBookingResponse>(result);
            Assert.Equal(result.Id, repair.Id);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenLeaseholdRepair_OnGetRepairBookingResponse_RepairBookingResponseIsReturnedWithRepair_Id()
#pragma warning restore CA1707
        {
            // Arrange
            repair.RepairType = RepairType.Leasehold;

            // Act
            var result = systemUnderTest.MapRepairBookingResponse(repair, false);

            // Assert
            Assert.IsType<RepairBookingResponse>(result);
            Assert.Equal(result.Id, repair.Id);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenCommunalRepair_OnGetRepairBookingResponse_CommunalRepairBookingResponseIsReturnedWithRepairId()
#pragma warning restore CA1707
        {
            // Arrange
            repair.RepairType = RepairType.Communal;

            // Act
            var result = systemUnderTest.MapRepairBookingResponse(repair, true);

            // Assert
            Assert.IsType<RepairBookingResponseWithDays>(result);
            Assert.Equal(result.Id, repair.Id);
            Assert.Equal(result.DaysForRepair, daysForRepair);
        }
    }
}
