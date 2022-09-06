using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using HousingRepairsOnlineApi.UseCases;
using Moq;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.UseCasesTests
{
    public class RetrieveTriageJourneyOptionsUseCaseTests
    {
        private readonly RetrieveJourneyTriageOptionsUseCase systemUnderTest;
        private readonly Mock<ISoREngine> sorEngineMock = new();

        public RetrieveTriageJourneyOptionsUseCaseTests()
        {
            systemUnderTest = new RetrieveJourneyTriageOptionsUseCase(sorEngineMock.Object);
        }

        [Fact]
        public async Task GivenNoOptions_WhenExecuting_ThenReturnsEmpty()
        {
            // Arrange
            sorEngineMock.Setup(x => x.RepairTriageOptions())
                .Returns(Array.Empty<RepairTriageOption>());

            // Act
            var actual = await systemUnderTest.Execute();

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GivenOptions_WhenExecuting_ThenReturnsEmpty()
        {
            // Arrange
            sorEngineMock.Setup(x => x.RepairTriageOptions())
                .Returns(new[] { new RepairTriageOption() });

            // Act
            var actual = await systemUnderTest.Execute();

            // Assert
            actual.Should().NotBeEmpty();
        }
    }
}
