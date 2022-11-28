using System.Linq;
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
        private readonly RepairQueryHelper systemUnderTest;
        private readonly Mock<ContainerResponse> containerMock;
        private readonly Mock<FeedIterator<Repair>> feedIteratorMock;
        private const string MockPropertyRef = "1234";
        private const string RepairType = "Communal";

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
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null))
                .Returns(feedIteratorMock.Object);

            // Act
            var result = systemUnderTest.GetItemQueryIterator<Repair>(RepairType, MockPropertyRef);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                        u.QueryText.Contains("SELECT")
                )
                , It.IsAny<string>(), null));
        }

        [Fact]
#pragma warning disable CA1707
        public void Test_Query_Definition_Returned_With_Repair_Type_Parameter()
#pragma warning restore CA1707
        {
            // Arrange
            feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(false);
            containerMock
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null))
                .Returns(feedIteratorMock.Object);

            // Act
            systemUnderTest.GetItemQueryIterator<Repair>(RepairType, MockPropertyRef);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                    u.GetQueryParameters()[0].Name == "@repairType"
                    && u.GetQueryParameters()[0].Value.ToString() == RepairType
                ),
                It.IsAny<string>(), null));
        }

        [Fact]
#pragma warning disable CA1707
        public void Test_Query_Definition_Returned_With_Property_Ref_Parameter()
#pragma warning restore CA1707
        {
            // Arrange
            feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(false);
            containerMock
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null))
                .Returns(feedIteratorMock.Object);

            // Act
            systemUnderTest.GetItemQueryIterator<Repair>(RepairType, MockPropertyRef);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                    u.GetQueryParameters()[1].Name == "@propertyReference"
                    && u.GetQueryParameters()[1].Value.ToString() == MockPropertyRef
                ),
                It.IsAny<string>(), null));
        }

        [Fact]
#pragma warning disable CA1707
        public void Test_Query_Definition_Returned_With_ParametersDefined()
#pragma warning restore CA1707
        {
            // Arrange
            feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(false);
            containerMock
                .Setup(_ => _.Container.GetItemQueryIterator<Repair>("TEST", null, null))
                .Returns(feedIteratorMock.Object);

            // Act
            systemUnderTest.GetItemQueryIterator<Repair>(RepairType, MockPropertyRef);

            // Assert
            containerMock.Verify(m => m.Container.GetItemQueryIterator<Repair>(
                It.Is<QueryDefinition>(u =>
                    u.GetQueryParameters().Count > 0),
                It.IsAny<string>(), null));
        }
    }
}
