using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class SaveChangedRepairRequestUseCaseTests
    {
        private readonly Mock<IRepairStorageGateway> repairStorageGatewayMock;
        private readonly SaveChangedRepairRequestUseCase systemUnderTest;
        private DateTime startDateTime = new DateTime(2022, 01, 01, 8, 0, 0);
        private DateTime endDateTime = new DateTime(2022, 01, 01, 12, 0, 0);

        private RepairAvailability repairAvailability = new RepairAvailability()
        {
            StartDateTime = new DateTime(2022, 01, 01, 8, 0, 0),
            EndDateTime = new DateTime(2022, 01, 01, 12, 0, 0),
            Display = "Display Text"
        };

        public SaveChangedRepairRequestUseCaseTests()
        {
            repairStorageGatewayMock = new Mock<IRepairStorageGateway>();
            systemUnderTest = new SaveChangedRepairRequestUseCase(repairStorageGatewayMock.Object);
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidRepair_WhenExecuting_ThenExceptionIsThrown<T>(T exception,
            Repair repair) where T : Exception
#pragma warning restore xUnit1026
        {
            //Act
            Func<Task> act = async () => await systemUnderTest.Execute(
                repair
            );

            //Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Fact]
        public async void GivenARepair_WhenExecuting_ThenRepairStorageGatewayModifyRepairIsCalledWithModifiedRepair()
        {
            // Arrange
            var repair = new Repair { Time = repairAvailability };
            repairStorageGatewayMock.Setup(x => x.ModifyRepair(It.IsAny<Repair>()));

            // Act
            await systemUnderTest.Execute(repair);

            // Assert
            repairStorageGatewayMock.Verify(x => x.ModifyRepair(It.Is<Repair>(x =>
                DateTime.Compare(x.Time.StartDateTime, startDateTime) == 0 && DateTime.Compare(x.Time.EndDateTime, endDateTime) == 0)), Times.Once);
        }
    }
}
