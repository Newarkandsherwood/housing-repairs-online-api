using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using FluentAssertions;
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
        private const string RepairId = "repairId";
        private const string Postcode = "postcode";

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
               default
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
                default
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

        [Theory]
        [MemberData(nameof(InvalidRepairTypesParameter))]
        public async void GivenInvalidRepairTypesParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
            T exception, IEnumerable<string> repairTypes) where T : ArgumentException
        {
            // Arrange

            // Act
            var act = () => azureStorageGateway.SearchByPostcodeAndId(repairTypes, Postcode, RepairId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static TheoryData<ArgumentException, IEnumerable<string>> InvalidRepairTypesParameter() =>
            new() {
                { new ArgumentNullException(), null },
                { new ArgumentException(), Enumerable.Empty<string>() },
            };

        [Theory]
        [MemberData(nameof(InvalidStringParameter))]
        public async void GivenInvalidPostcodeParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
            T exception, string postcode) where T : ArgumentException
        {
            // Arrange

            // Act
            var act = () => azureStorageGateway.SearchByPostcodeAndId(new[] { "repairType" }, postcode, RepairId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidStringParameter))]
        public async void GivenInvalidRepairIdParameter_WhenExecuting_ThenAppropriateArgumentExceptionIsThrown<T>(
            T exception, string repairId) where T : ArgumentException
        {
            // Arrange

            // Act
            var act = () => azureStorageGateway.SearchByPostcodeAndId(new[] { "repairType" }, Postcode, repairId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static TheoryData<ArgumentException, string> InvalidStringParameter() =>
            new() {
                { new ArgumentNullException(), null },
                { new ArgumentException(), string.Empty },
                { new ArgumentException(), " " },
            };

        [Fact]
        public async void GivenRepairsAreNotFound_WhenSearchingByPostcodeAndId_ThenNoRepairRequestSummariesAreReturned()
        {
            // Arrange
            var feedResponseRepairMock = new Mock<FeedResponse<Repair>>();
            feedResponseRepairMock.Setup(x => x.GetEnumerator())
                .Returns(new List<Repair>().GetEnumerator());

            var feedIteratorRepairMock = new Mock<FeedIterator<Repair>>();
            feedIteratorRepairMock.SetupSequence(x => x.HasMoreResults).Returns(true).Returns(false);
            feedIteratorRepairMock.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseRepairMock.Object);

            var repairTypes = new[] { "test" };
            repairQueryHelper.Setup(x => x.GetRepairSearchIterator(repairTypes, Postcode, RepairId))
                .Returns<IEnumerable<string>, string, string>((_, _, _) =>
                    feedIteratorRepairMock.Object);

            // Act
            var actual = await azureStorageGateway.SearchByPostcodeAndId(repairTypes, Postcode, RepairId);

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async void
            GivenRepairsAreFound_WhenSearchingByPostcodeAndId_ThenAppropriateRepairRequestSummariesAreReturned()
        {
            // Arrange
            var address = new RepairAddress { Display = "1 Fake Lane, Fake City, FA4 3XX", LocationId = "1" };
            var description = new RepairDescription { Text = "description text" };
            var issue = new RepairIssue { Display = "An Issue", Value = "issue" };
            var location = new RepairLocation { Display = "A location", Value = "location" };
            var problem = new RepairProblem { Display = "A problem", Value = "problem" };

            var feedResponseRepairMock = new Mock<FeedResponse<Repair>>();
            var repair = new Repair
            {
                Address = address,
                Description = description,
                Issue = issue,
                Location = location,
                Problem = problem
            };
            feedResponseRepairMock.Setup(x => x.GetEnumerator())
                .Returns(new List<Repair> { repair }.GetEnumerator());

            var feedIteratorRepairMock = new Mock<FeedIterator<Repair>>();
            feedIteratorRepairMock.SetupSequence(x => x.HasMoreResults).Returns(true).Returns(false);
            feedIteratorRepairMock.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseRepairMock.Object);

            var repairTypes = new[] { "test" };
            repairQueryHelper.Setup(x => x.GetRepairSearchIterator(repairTypes, Postcode, RepairId))
                .Returns<IEnumerable<string>, string, string>((_, _, _) =>
                    feedIteratorRepairMock.Object);

            var expected = new RepairRequestSummary
            {
                Address = address,
                Description = description,
                Issue = issue,
                Location = location,
                Problem = problem
            };

            // Act
            var actual = await azureStorageGateway.SearchByPostcodeAndId(repairTypes, Postcode, RepairId);

            // Assert
            var repairRequestSummaries = actual.ToArray();
            repairRequestSummaries.Should().HaveCount(1);
            var actualRepairRequestSummary = repairRequestSummaries.Single();
            actualRepairRequestSummary.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void GivenARepair_WhenCancellingRepair_RepairCancelled()
        {
            // Arrange
            var repairId = "ABCD1234";
            var mockItemResponse = new Mock<ItemResponse<Repair>>();
            mockCosmosContainer.Setup(_ => _.PatchItemAsync<Repair>(
                repairId,
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                It.IsAny<CancellationToken>()

            )).ReturnsAsync(mockItemResponse.Object);

            // Act
            await azureStorageGateway.CancelRepair(repairId);

            // Assert
            mockCosmosContainer.Verify(_ => _.PatchItemAsync<Repair>(repairId,
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}
