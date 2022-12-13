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
    public class ChangeRepairRequestUseCaseTests
    {
        private readonly Mock<IRepairStorageGateway> repairStorageGatewayMock;
        private readonly ChangeRepairRequestUseCase systemUnderTest;
        private DateTime startDateTime = new DateTime(2022, 01, 01, 8, 0, 0);
        private DateTime endDateTime = new DateTime(2022, 01, 01, 12, 0, 0);

        public ChangeRepairRequestUseCaseTests()
        {
            repairStorageGatewayMock = new Mock<IRepairStorageGateway>();
            systemUnderTest = new ChangeRepairRequestUseCase(repairStorageGatewayMock.Object);
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
                repair, startDateTime, endDateTime
            );

            //Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Fact]
        public async void GivenARepair_WhenExecuting_ThenRepairStorageGatewayModifyRepairIsCalledWithModifiedRepair()
        {
            // Arrange
            var repairAvailability = new RepairAvailability()
            {
                StartDateTime = new DateTime(2022, 1, 1),
                EndDateTime = new DateTime(2022, 1, 2),
            };
            var repair = new Repair { Time = repairAvailability };
            repairStorageGatewayMock.Setup(x => x.ModifyRepair(It.IsAny<Repair>()));

            // Act
            await systemUnderTest.Execute(repair, startDateTime, endDateTime);

            // Assert
            repairStorageGatewayMock.Verify(x => x.ModifyRepair(It.Is<Repair>(x =>
                DateTime.Compare(x.Time.StartDateTime, startDateTime) == 0 && DateTime.Compare(x.Time.EndDateTime, endDateTime) == 0)), Times.Once);
        }
    }
}
