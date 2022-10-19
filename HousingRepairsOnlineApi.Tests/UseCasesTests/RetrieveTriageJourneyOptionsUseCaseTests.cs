using System;
using System.Collections.Generic;
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
        private readonly Mock<ISorEngineResolver> sorEngineResolverMock = new();
        private readonly Mock<IEarlyExitRepairTriageOptionMapper> earlyExitRepairTriageOptionMapperMock = new();

        public RetrieveTriageJourneyOptionsUseCaseTests()
        {
            earlyExitRepairTriageOptionMapperMock.Setup(x =>
                    x.MapRepairTriageOption(It.IsAny<IEnumerable<RepairTriageOption>>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns<IEnumerable<RepairTriageOption>, string, string, string, string>(
                    (repairTriageOptions, emergencyValue, notEligibleNonEmergency, unableToBook, contactUs) =>
                        repairTriageOptions);
            sorEngineResolverMock.Setup(x => x.Resolve(It.IsAny<string>())).Returns(sorEngineMock.Object);
            systemUnderTest = new RetrieveJourneyTriageOptionsUseCase(sorEngineResolverMock.Object, earlyExitRepairTriageOptionMapperMock.Object);
        }

        [Fact]
        public async Task GivenNoOptions_WhenExecuting_ThenReturnsEmpty()
        {
            // Arrange
            sorEngineMock.Setup(x => x.RepairTriageOptions())
                .Returns(Array.Empty<RepairTriageOption>());

            // Act
            var actual = await systemUnderTest.Execute(string.Empty, string.Empty, string.Empty, string.Empty);

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task GivenOptions_WhenExecuting_ThenReturnsNonEmpty()
        {
            // Arrange
            sorEngineMock.Setup(x => x.RepairTriageOptions())
                .Returns(new[] { new RepairTriageOption() });

            // Act
            var actual = await systemUnderTest.Execute(string.Empty, string.Empty, string.Empty, string.Empty);

            // Assert
            actual.Should().NotBeEmpty();
        }
    }
}
