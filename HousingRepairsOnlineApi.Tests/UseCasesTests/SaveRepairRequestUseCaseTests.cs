using System;
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
    public class SaveRepairRequestUseCaseTests
    {
        private const string RepairTypeParameterValue = RepairType.Tenant;

        private readonly SaveRepairRequestUseCase systemUnderTest;
        private readonly Mock<IRepairRequestToRepairMapper> mockRepairRequestToRepairMapper;
        private readonly Mock<IRepairStorageGateway> mockCosmosGateway;
        private readonly Mock<IBlobStorageGateway> mockAzureStorageGateway;
        private const string repairType = "Communal";
        private const string repairId = "RepairId";

        public SaveRepairRequestUseCaseTests()
        {
            mockCosmosGateway = new Mock<IRepairStorageGateway>();
            mockRepairRequestToRepairMapper = new Mock<IRepairRequestToRepairMapper>();
            mockAzureStorageGateway = new Mock<IBlobStorageGateway>();
            systemUnderTest = new SaveRepairRequestUseCase(
                mockCosmosGateway.Object,
                mockAzureStorageGateway.Object,
                mockRepairRequestToRepairMapper.Object
            );
        }

        [Fact]
        public async void GivenARepairRequestWithImageARepairIsSaved()
        {
            const string Location = "kitchen";
            const string Problem = "cupboards";
            const string Issue = "doorHangingOff";
            const string RepairCode = "N373049";
            const string Base64Img = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw1AUhU9TpaJVB4uIOGSoThZERRy1CkWoEGqFVh1MXvojNGlIUlwcBdeCgz+LVQcXZ10dXAVB8AfE0clJ0UVKvC8ptIjxwuN9nHfP4b37AKFWYprVNgZoum2mEnExk10RQ68IIIQe9KNLZpYxK0lJ+NbXPXVT3cV4ln/fn9Wt5iwGBETiGWaYNvE68dSmbXDeJ46woqwSnxOPmnRB4keuKx6/cS64LPDMiJlOzRFHiMVCCystzIqmRjxJHFU1nfKFjMcq5y3OWqnCGvfkLwzn9OUlrtMaQgILWIQEEQoq2EAJNmK066RYSNF53Mc/6Polcink2gAjxzzK0CC7fvA/+D1bKz8x7iWF40D7i+N8DAOhXaBedZzvY8epnwDBZ+BKb/rLNWD6k/RqU4seAb3bwMV1U1P2gMsdYODJkE3ZlYK0hHweeD+jb8oCfbdA56o3t8Y5Th+ANM0qeQMcHAIjBcpe83l3R+vc/u1pzO8H+I9yds6VEEcAAAAJcEhZcwAALiMAAC4jAXilP3YAAAAHdElNRQfmAQcOFjXsyx/IAAAAGXRFWHRDb21tZW50AENyZWF0ZWQgd2l0aCBHSU1QV4EOFwAAAAxJREFUCNdj0HiTBAACtgF3wqeo5gAAAABJRU5ErkJggg==";
            const string FileExtension = "png";
            const string ImgUrl = "http://img.png";
            const string LocationText = "LocationText";
            const string Description = "Description";

            var repairRequest = new RepairRequest
            {
                Location = new RepairLocation { Value = Location },
                Problem = new RepairProblem { Value = Problem },
                Issue = new RepairIssue { Value = Issue },
                Description = new RepairDescriptionRequest { Base64Img = Base64Img, FileExtension = FileExtension, LocationText = LocationText, Text = Description }
            };

            var repair = new Repair
            {
                RepairType = RepairTypeParameterValue,
                Location = new RepairLocation { Value = Location },
                Problem = new RepairProblem { Value = Problem },
                Issue = new RepairIssue { Value = Issue },
                SOR = RepairCode,
                Description = new RepairDescription { Base64Image = Base64Img, Text = LocationText + " " + Location }
            };
            mockRepairRequestToRepairMapper.Setup(x => x.Map(It.IsAny<RepairRequest>(), It.IsAny<string>(), It.IsAny<string>())).Returns(repair);

            mockAzureStorageGateway.Setup(x => x.UploadBlob(Base64Img, FileExtension))
                .ReturnsAsync(ImgUrl);

            mockCosmosGateway.Setup(x => x.AddRepair(It.IsAny<Repair>()))
                .ReturnsAsync((Repair r) => r);

            var _ = await systemUnderTest.Execute(RepairTypeParameterValue, repairRequest, repairId);

            mockAzureStorageGateway.Verify(x => x.UploadBlob(Base64Img, FileExtension), Times.Once);
            mockRepairRequestToRepairMapper.Verify(x => x.Map(repairRequest, RepairTypeParameterValue, It.IsAny<string>()), Times.Once);
            mockCosmosGateway.Verify(x => x.AddRepair(It.Is<Repair>(p => p.Status == RepairStatus.Scheduled)), Times.Once);
        }

        [Fact]
        public async void GivenARepairRequestWithoutAnImageARepairIsSaved()
        {
            const string Location = "kitchen";
            const string Problem = "cupboards";
            const string Issue = "doorHangingOff";
            const string RepairCode = "N373049";
            const string Priority = "priority";
            var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = RepairCode, Priority = Priority };

            var repairRequest = new RepairRequest()
            {
                Location = new RepairLocation()
                {
                    Value = Location
                },
                Problem = new RepairProblem()
                {
                    Value = Problem,
                },
                Issue = new RepairIssue()
                {
                    Value = Issue
                },
                Description = new RepairDescriptionRequest()
                {
                    Text = "Lorem ipsum"
                }

            };
            var repair = new Repair
            {
                Location = new RepairLocation { Value = Location },
                Problem = new RepairProblem { Value = Problem },
                Issue = new RepairIssue { Value = Issue },
                Description = new RepairDescription { Text = "Lorem ipsum" }
            };

            mockRepairRequestToRepairMapper.Setup(x => x.Map(repairRequest, RepairTypeParameterValue, It.IsAny<string>())).Returns(repair);

            mockCosmosGateway.Setup(x => x.AddRepair(It.IsAny<Repair>()))
                .ReturnsAsync((Repair r) => r);

            var _ = await systemUnderTest.Execute(RepairTypeParameterValue, repairRequest, repairId);

            mockRepairRequestToRepairMapper.Verify(x => x.Map(repairRequest, RepairTypeParameterValue, It.IsAny<string>()), Times.Once);
            mockCosmosGateway.Verify(x => x.AddRepair(It.Is<Repair>(p => p.Status == RepairStatus.Scheduled)), Times.Once);
            mockAzureStorageGateway.Verify(x => x.UploadBlob(null, null), Times.Never());
        }

        [Fact]
        public async void GivenARepairRequestARepairIsSavedWithScheduledStatus()
        {
            const string Location = "kitchen";
            const string Problem = "cupboards";
            const string Issue = "doorHangingOff";
            const string RepairCode = "N373049";
            const string Priority = "priority";
            var repairTriageDetails = new RepairTriageDetails { ScheduleOfRateCode = RepairCode, Priority = Priority };

            var repairRequest = new RepairRequest()
            {
                Location = new RepairLocation()
                {
                    Value = Location
                },
                Problem = new RepairProblem()
                {
                    Value = Problem,
                },
                Issue = new RepairIssue()
                {
                    Value = Issue
                },
                Description = new RepairDescriptionRequest()
                {
                    Text = "Lorem ipsum"
                }

            };
            var repair = new Repair
            {
                Location = new RepairLocation { Value = Location },
                Problem = new RepairProblem { Value = Problem },
                Issue = new RepairIssue { Value = Issue },
                Description = new RepairDescription { Text = "Lorem ipsum" }
            };

            mockRepairRequestToRepairMapper.Setup(x => x.Map(repairRequest, RepairTypeParameterValue, It.IsAny<string>())).Returns(repair);

            mockCosmosGateway.Setup(x => x.AddRepair(It.IsAny<Repair>()))
                .ReturnsAsync((Repair r) => r);

            var _ = await systemUnderTest.Execute(RepairTypeParameterValue, repairRequest, repairId);

            mockCosmosGateway.Verify(x => x.AddRepair(It.Is<Repair>(p => p.Status == RepairStatus.Scheduled)), Times.Once);
        }
        [Theory]
        [MemberData(nameof(Helpers.RepairTypeTestData.InvalidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
#pragma warning disable xUnit1026
        public async void GivenAnInvalidRepairType_WhenExecute_ThenExceptionIsThrown<T>(T exception, string repairTypeParameter) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.Execute(repairTypeParameter, default, repairId);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        [Theory]
        [MemberData(nameof(Helpers.RepairTypeTestData.ValidRepairTypeArgumentTestData), MemberType = typeof(Helpers.RepairTypeTestData))]
        public void GivenValidRepairTypeParameter_WhenResolving_ThenExceptionIsNotThrown(string repairTypeParameter)
        {
            // Arrange

            // Act
            Action act = () => _ = systemUnderTest.Execute(repairTypeParameter, default, repairId);

            // Assert
            act.Should().NotThrow<ArgumentException>();
        }
    }
}
