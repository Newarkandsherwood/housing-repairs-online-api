using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class RepairQueryHelperTests
    {
        private RepairQueryHelper systemUnderTest;
        private Mock<ContainerResponse> containerMock;
        private Mock<FeedIterator<Repair>> feedIteratorMock;
        private const string MockPostcode = "NG21 9LQ";
        private const string repairType = "Communal";

        public RepairQueryHelperTests()
        {
            containerMock = new Mock<ContainerResponse>();
            systemUnderTest = new RepairQueryHelper(containerMock.Object);
            feedIteratorMock = new Mock<FeedIterator<Repair>>();
        }

        [Fact]
#pragma warning disable CA1707
        public void Test_Query_Definition_Returned_With_Querytext_Containing_SELECT()
#pragma warning restore CA1707
        {
            // Arrange
            feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(false);
            containerMock
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null)) //It.IsAny<string>()
                .Returns(feedIteratorMock.Object);

            // Act
            var result = systemUnderTest.GetItemQueryIterator<Repair>(repairType, MockPostcode);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                        u.QueryText.Contains("SELECT")
                )
                , It.IsAny<string>(), null));
        }

        [Fact]
#pragma warning disable CA1707
        public void Test_Query_Definition_Returned_With_Parameter()
#pragma warning restore CA1707
        {
            // Arrange
            feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(false);
            containerMock
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null))
                .Returns(feedIteratorMock.Object);

            // Act
            systemUnderTest.GetItemQueryIterator<Repair>(repairType, MockPostcode);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                    u.GetQueryParameters()[0].Name == "@propertyReference"
                    && u.GetQueryParameters()[0].Value.ToString() == MockPostcode
                ),
                It.IsAny<string>(), null));
        }
    }
}
