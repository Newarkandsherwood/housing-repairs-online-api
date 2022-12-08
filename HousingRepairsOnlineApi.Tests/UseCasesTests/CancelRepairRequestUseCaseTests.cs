using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class CancelRepairRequestUseCaseTests
    {
        private readonly Mock<IRepairStorageGateway> repairStorageGatewayMock;
        private readonly CancelRepairRequestUseCase systemUnderTest;

        public CancelRepairRequestUseCaseTests()
        {
            repairStorageGatewayMock = new Mock<IRepairStorageGateway>();
            systemUnderTest = new CancelRepairRequestUseCase(repairStorageGatewayMock.Object);
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
        public async void GivenARepair_WhenExecuting_ThenRepairStorageGatewayCancelRepairIsCalled()
        {
            // Arrange
            var repair = new Repair();
            repairStorageGatewayMock.Setup(x => x.CancelRepair(It.IsAny<Repair>()));

            // Act
            await systemUnderTest.Execute(repair);

            // Assert
            repairStorageGatewayMock.Verify(x => x.CancelRepair(It.IsAny<Repair>()), Times.Once);
        }
    }
}
