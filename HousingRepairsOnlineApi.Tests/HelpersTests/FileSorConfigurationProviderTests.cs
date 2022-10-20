using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class FileSorConfigurationProviderTests
    {
        private readonly MockFileSystem fileSystemMock = new();
        private readonly FileSorConfigurationProvider systemUnderTest;
        private const string configFilePath = "SoRConfig.json";

        public FileSorConfigurationProviderTests()
        {
            systemUnderTest = new FileSorConfigurationProvider(configFilePath, fileSystemMock);
        }

        [Fact]
        public void GivenValidSorConfigPathArgument_WhenAddingSoREngineToServices_ThenSoREngineIsRegistered()
        {
            // Arrange
            var config = string.Empty;
            fileSystemMock.AddFile(configFilePath, new MockFileData(config));

            // Act
            Action act = () => _ = systemUnderTest.ConfigurationValue();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void GivenPathToMissingSorConfigPathArgument_WhenMapToRepairTriageDetailsCalled_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange

            // Act
            Action act = () => systemUnderTest.ConfigurationValue();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithInnerException<FileNotFoundException>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public void GivenInvalidSorConfigPathArgument_WhenConstructing_ThenExceptionIsThrown<T>(T exception, string sorConfigPath) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Action act = () => _ = new FileSorConfigurationProvider(sorConfigPath);

            // Assert
            act.Should().Throw<T>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }
    }
}
