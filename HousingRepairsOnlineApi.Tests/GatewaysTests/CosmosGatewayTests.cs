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

            azureStorageGateway = new CosmosGateway(mockCosmosContainer.Object, repairQueryHelper.Object);
        }

        [Fact]
        public async void AnItemIsAdded()
        {
            var dummyRepair = new Repair();

            var responseMock = new Mock<ItemResponse<Repair>>();
            responseMock.Setup(_ => _.Resource).Returns(dummyRepair);

            // Arrange
            mockCosmosContainer.Setup(_ => _.CreateItemAsync(
               dummyRepair,
               null,
               null,
               default
               )).ReturnsAsync(responseMock.Object);

            var actual = await azureStorageGateway.AddRepair(dummyRepair);

            // Assert
            mockCosmosContainer.VerifyAll();
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
            new()
            {
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
            new()
            {
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
            repairQueryHelper.Setup(x => x.GetRepairSearchIterator(repairTypes, Postcode, RepairId, false))
                .Returns<IEnumerable<string>, string, string, bool>((_, _, _, _) =>
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
            repairQueryHelper.Setup(x => x.GetRepairSearchIterator(repairTypes, Postcode, RepairId, false))
                .Returns<IEnumerable<string>, string, string, bool>((_, _, _, _) =>
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
            mockCosmosContainer.Setup(_ => _.ReplaceItemAsync(
                It.IsAny<Repair>(),
                repairId,
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(mockItemResponse.Object);

            var repair = new Repair
            {
                Id = repairId,
                Status = RepairStatus.Cancelled
            };

            // Act
            await azureStorageGateway.ModifyRepair(repair);

            // Assert
            mockCosmosContainer.Verify(_ => _.ReplaceItemAsync<Repair>(It.Is<Repair>(r => r.Status == RepairStatus.Cancelled),
                repairId,
                null,
                null,
                It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}
