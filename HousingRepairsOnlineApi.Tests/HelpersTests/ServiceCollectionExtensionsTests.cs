using System;
using System.Collections.Generic;
using FluentAssertions;
using HousingRepairsOnlineApi.Domain;
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

            var jsonConfig = @"[{
                ""value"": ""kitchen"",
                ""display"": ""Kitchen"",
                ""options"": [{
                        ""value"": ""cupboards"",
                        ""display"": ""Cupboards, including damaged..."",
                        ""options"": [{
                            ""value"": ""doorHangingOff"",
                            ""display"": ""Hanging Door"",
                            ""sorCode"": ""SoR Code 1""
                        }]
                    }]
                },
                {
                ""value"": ""bathroom"",
                ""display"": ""Bathroom"",
                ""options"": [{
                    ""value"": ""bath"",
                    ""display"": ""Bath, including taps"",
                    ""options"": [{
                        ""value"": ""taps"",
                        ""display"": ""Bath taps"",
                        ""sorCode"": ""SoR Code 1""
                    }]
                }]
            }]";
            sorConfigurationProviderMock.Setup(x => x.ConfigurationValue()).Returns(jsonConfig);

            // Act
            ServiceCollectionExtensions.AddSoREngine(serviceCollection, sorConfigurationProviderMock.Object);

            // Assert
            serviceCollectionMock.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(InvalidJsonSorConfigTestData))]
#pragma warning disable xUnit1026
        public void GivenInvalidJsonSorConfig_WhenAddingSoREngineToServices_ThenExceptionIsThrown<T>(T exception,
            string configJson) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var serviceCollectionMock = new Mock<IServiceCollection>();
            var serviceCollection = serviceCollectionMock.Object;

            sorConfigurationProviderMock.Setup(x => x.ConfigurationValue()).Returns(configJson);

            // Act
            Action act = () =>
                ServiceCollectionExtensions.AddSoREngine(serviceCollection, sorConfigurationProviderMock.Object);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithInnerException<T>();
        }

        [Theory]
        [MemberData(nameof(InvalidJsonSorConfigTestData))]
#pragma warning disable xUnit1026
        public void GivenInvalidJsonSorConfig_WhenParsingSorConfigurationJson_ThenExceptionIsThrown<T>(T exception,
            string configJson) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Action act = () => ServiceCollectionExtensions.ParseSorConfigurationJson(configJson);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithInnerException<T>();
        }

        [Theory]
        [MemberData(nameof(ValidSorConfigurationForCreatingJourneyRepairTriageOptions))]
        public void GivenSorConfigurations_WhenGeneratingJourneyRepairTriageOptions_ThenRepairTriageOptionsReturned(
            IEnumerable<SorConfiguration> sorConfigurations, IEnumerable<RepairTriageOption> expected)
        {
            // Arrange

            // Act
            var actual = ServiceCollectionExtensions.GenerateJourneyRepairTriageOptions(
                sorConfigurations);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(ValidSorConfigurationForCreatingSorMapping))]
        public void GivenSorConfigurations_WhenGeneratingSorMapping_ThenCorrectMappingCreated(
            IEnumerable<SorConfiguration> sorConfigurations, IDictionary<string, IDictionary<string, dynamic>> expected)
        {
            // Arrange

            // Act
            var actual = ServiceCollectionExtensions.GenerateSorMapping(sorConfigurations);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<JsonException, string> InvalidJsonSorConfigTestData()
        {
            return new TheoryData<JsonException, string>
            {
                { new JsonSerializationException(), @"{" },
                { new JsonReaderException(), @"}" },
                {
                    new JsonSerializationException(), @"{
                        ""kitchen"": {
                    }"
                },
                {
                    new JsonReaderException(), @"[{
                    ""value"": ""kitchen"",
                    ""display"": ""Kitchen"",
                    ""options"": [{
                            ""value"": ""cupboards"",
                            ""display"": ""Cupboards, including damaged..."",
                            ""options"": [{
                                ""value"": ""doorHangingOff"",
                                ""display"": ""Hanging Door"",
                                ""sorCode"": ""SoR Code 1""
                            }]
                        }]
                    {
                    ""value"": ""bathroom"",
                    ""display"": ""Bathroom"",
                    ""options"": [{
                        ""value"": ""bath"",
                        ""display"": ""Bath, including taps"",
                        ""options"": [{
                            ""value"": ""taps"",
                            ""display"": ""Bath taps"",
                            ""sorCode"": ""SoR Code 1""
                        }]
                    }]
                }]"
                },
            };
        }

        public static TheoryData<IEnumerable<SorConfiguration>, IEnumerable<RepairTriageOption>> ValidSorConfigurationForCreatingJourneyRepairTriageOptions()
        {
            var value = "value1";
            var display = "display1";
            var value2 = "value2";
            var display2 = "display2";
            var value3 = "value3";
            var display3 = "display3";

            return new TheoryData<IEnumerable<SorConfiguration>, IEnumerable<RepairTriageOption>>()
            {
                { Array.Empty<SorConfiguration>(), Array.Empty<RepairTriageOption>() },
                {
                    new[] { new SorConfiguration { Value = value, Display = display, SorCode = "sorCode" } },
                    new[] { new RepairTriageOption { Value = value, Display = display, } }
                },
                {
                    new[]
                    {
                        new SorConfiguration
                        {
                            Value = value,
                            Display = display,
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Value = value2, Display = display2, SorCode = "sorCode"
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new RepairTriageOption
                        {
                            Value = value,
                            Display = display,
                            Options = new[] { new RepairTriageOption { Value = value2, Display = display2 } }
                        }
                    }
                },
                {
                    new[]
                    {
                        new SorConfiguration
                        {
                            Value = value,
                            Display = display,
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Value = value2,
                                    Display = display2,
                                    Options = new[]
                                    {
                                        new SorConfiguration
                                        {
                                            Value = value3,
                                            Display = display3,
                                            SorCode = "sorCode"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new[]
                    {
                        new RepairTriageOption
                        {
                            Value = value,
                            Display = display,
                            Options = new[]
                            {
                                new RepairTriageOption
                                {
                                    Value = value2,
                                    Display = display2,
                                    Options = new[]
                                    {
                                        new RepairTriageOption
                                        {
                                            Value = value3, Display = display3
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            };
        }

        public static TheoryData<IEnumerable<SorConfiguration>, IDictionary<string, IDictionary<string, dynamic>>>
            ValidSorConfigurationForCreatingSorMapping()
        {
            var value = "value1";
            var display = "display1";
            var sorCode = "sorCode";
            var value2 = "value2";
            var display2 = "display2";
            var value3 = "value3";
            var display3 = "display3";

            return new TheoryData<IEnumerable<SorConfiguration>, IDictionary<string, IDictionary<string, dynamic>>>
            {
                { Array.Empty<SorConfiguration>(), new Dictionary<string, IDictionary<string, dynamic>>() },
                {
                    new[]
                    {
                        new SorConfiguration
                        {
                            Value = value,
                            Display = display,
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Value = value2, Display = display2, SorCode = sorCode
                                }
                            }
                        }
                    },
                    new Dictionary<string, IDictionary<string, dynamic>>
                    {
                        { value, new Dictionary<string, dynamic> { { value2, sorCode } } }
                    }
                },
                {
                    new[]
                    {
                        new SorConfiguration
                        {
                            Value = value,
                            Display = display,
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Value = value2,
                                    Display = display2,
                                    Options = new[]
                                    {
                                        new SorConfiguration
                                        {
                                            Value = value3,
                                            Display = display3,
                                            SorCode = sorCode
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new Dictionary<string, IDictionary<string, dynamic>>
                    {
                        {
                            value,
                            new Dictionary<string, dynamic>
                            {
                                { value2, new Dictionary<string, string> { { value3, sorCode } } }
                            }
                        }
                    }
                },
            };
        }
    }
}
