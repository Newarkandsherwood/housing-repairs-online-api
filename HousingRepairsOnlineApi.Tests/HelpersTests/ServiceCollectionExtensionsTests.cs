using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using ServiceCollectionExtensions = HousingRepairsOnlineApi.Helpers.ServiceCollectionExtensions;

namespace HousingRepairsOnlineApi.Tests.HelpersTests
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly Mock<ISorConfigurationProvider> sorConfigurationProviderMock = new();

        [Fact]
        public void GivenValidSorConfigPathArgument_WhenAddingSoREngineToServices_ThenSoREngineIsRegistered()
        {
            // Arrange
            var serviceCollectionMock = new Mock<IServiceCollection>();
            serviceCollectionMock.Setup(x =>
                x.Add(It.Is<ServiceDescriptor>(serviceDescriptor =>
                        serviceDescriptor.ServiceType == typeof(ISoREngine) &&
                        serviceDescriptor.ImplementationFactory != null
                    )
                )
            );
            var serviceCollection = serviceCollectionMock.Object;

            var jsonConfig = @"{
                ""kitchen"": {
                        ""cupboards"": {
                        ""doorHangingOff"": ""SoR Code 1""
                    }
                },
                ""bathroom"": {
                    ""bath"": {
                        ""taps"": ""SoR Code 1""
                    }
                }
            }";
            sorConfigurationProviderMock.Setup(x => x.ConfigurationValue()).Returns(jsonConfig);

            // Act
            ServiceCollectionExtensions.AddSoREngine(serviceCollection, sorConfigurationProviderMock.Object);

            // Assert
            serviceCollectionMock.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(InvalidJsonSorConfigTestData))]
#pragma warning disable xUnit1026
        public void GivenInvalidJsonSorConfig_WhenAddingSoREngineToServices_ThenExceptionIsThrown<T>(T expection, string configJson) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var serviceCollection = serviceCollectionMock.Object;

            sorConfigurationProviderMock.Setup(x => x.ConfigurationValue()).Returns(configJson);

            // Act
            Action act = () => ServiceCollectionExtensions.AddSoREngine(serviceCollection, sorConfigurationProviderMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithInnerException<T>();
        }

        public static IEnumerable<object[]> InvalidJsonSorConfigTestData()
        {
            yield return new object[] { new JsonSerializationException(), @"{" };
            yield return new object[] { new JsonReaderException(), @"}" };
            yield return new object[] { new JsonSerializationException(), @"
                {
                    ""kitchen"": {
                }"
            };
            yield return new object[] { new JsonReaderException(), @"
                {
                    ""kitchen"": {
                            ""cupboards"": {
                            ""doorHangingOff"": ""SoR Code 1""
                        }
                    ""bathroom"": {
                        ""bath"": {
                            ""taps"": ""SoR Code 1""
                        }
                    }
                }"
            };

        }
    }
}
