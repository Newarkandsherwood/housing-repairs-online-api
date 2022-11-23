using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class RepairBookingResponseHelperTests
    {
        private readonly RepairBookingResponseHelper _systemUnderTest;
        private readonly Repair _repair = new();
        private readonly int _daysForRepair = 30;

        public RepairBookingResponseHelperTests()
        {
            var repairDayWindowHelperMock = new Mock<IRepairDayWindowHelper>();
            repairDayWindowHelperMock.Setup(_ => _.GetDaysForRepair(It.IsAny<Repair>())).Returns(_daysForRepair);
            _repair.Id = "repairId";
            _systemUnderTest = new RepairBookingResponseHelper(repairDayWindowHelperMock.Object);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenATenantRepair_OnGetRepairBookingResponse_RepairBookingResponseIsReturnedWithRepairId()
#pragma warning restore CA1707
        {
            // Arrange
            _repair.RepairType = RepairType.Tenant;

            // Act
            var result = _systemUnderTest.GetRepairBookingResponse(_repair);

            // Assert
            Assert.IsType<RepairBookingResponse>(result);
            Assert.Equal(result.Id, _repair.Id);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenLeaseholdRepair_OnGetRepairBookingResponse_RepairBookingResponseIsReturnedWithRepair_Id()
#pragma warning restore CA1707
        {
            // Arrange
            _repair.RepairType = RepairType.Leasehold;

            // Act
            var result = _systemUnderTest.GetRepairBookingResponse(_repair);

            // Assert
            Assert.IsType<RepairBookingResponse>(result);
            Assert.Equal(result.Id, _repair.Id);
        }

        [Fact]
#pragma warning disable CA1707
        public void GivenCommunalRepair_OnGetRepairBookingResponse_CommunalRepairBookingResponseIsReturnedWithRepairId()
#pragma warning restore CA1707
        {
            // Arrange
            _repair.RepairType = RepairType.Communal;

            // Act
            var result = _systemUnderTest.GetRepairBookingResponse(_repair);

            // Assert
            Assert.IsType<CommunalRepairBookingResponse>(result);
            Assert.Equal(result.Id, _repair.Id);
            Assert.Equal(result.DaysForRepair, _daysForRepair);
        }
    }
}
