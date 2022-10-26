using System.Collections.Generic;
using System.Net;
using System.Threading;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Gateways;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.GatewaysTests
{
    public class CosmosGatewayTests
    {
        private readonly CosmosGateway azureStorageGateway;
        private readonly Mock<Container> mockCosmosContainer;
        private readonly Mock<IIdGenerator> mockIdGenerator;
        private readonly Mock<IRepairQueryHelper> repairQueryHelper;

        public CosmosGatewayTests()
        {
            mockCosmosContainer = new Mock<Container>();
            mockIdGenerator = new Mock<IIdGenerator>();
            repairQueryHelper = new Mock<IRepairQueryHelper>();

            azureStorageGateway = new CosmosGateway(mockCosmosContainer.Object, mockIdGenerator.Object, repairQueryHelper.Object);
        }

        [Fact]
        public async void AnItemIsAdded()
        {
            var repairId = "ABCD1234";
            var dummyRepair = new Repair();

            var responseMock = new Mock<ItemResponse<Repair>>();
            responseMock.Setup(_ => _.Resource).Returns(dummyRepair);
            mockIdGenerator.Setup(_ => _.Generate()).Returns(repairId);

            // Arrange
            mockCosmosContainer.Setup(_ => _.CreateItemAsync(
               dummyRepair,
               null,
               null,
               default(CancellationToken)
               )).ReturnsAsync(responseMock.Object);

            var actual = await azureStorageGateway.AddRepair(dummyRepair);

            // Assert
            Assert.Equal(repairId, actual.Id);
            mockIdGenerator.Verify(_ => _.Generate(), Times.Once());
        }

        [Fact]
        public async void AnIdIsRegenerated()
        {
            var conflictId = "ABCD1234";
            var repairId = "1234ABCD";
            var dummyRepair = new Repair();

            var responseMock = new Mock<ItemResponse<Repair>>();
            responseMock.Setup(_ => _.Resource).Returns(dummyRepair);
            mockIdGenerator.Setup(_ => _.Generate()).Returns(
                new Queue<string>(new[] { conflictId, repairId }).Dequeue);

            mockCosmosContainer.Setup(_ => _.CreateItemAsync(
                dummyRepair,
                null,
                null,
                default(CancellationToken)
            )).Callback(() =>
                {
                    if (dummyRepair.Id == conflictId)
                        throw new CosmosException("Conflict", HttpStatusCode.Conflict, default, default, default);
                }).ReturnsAsync(responseMock.Object);

            var actual = await azureStorageGateway.AddRepair(dummyRepair);

            // Assert
            mockIdGenerator.Verify(_ => _.Generate(), Times.Exactly(2));
            Assert.Equal(repairId, actual.Id);
        }
    }
}
