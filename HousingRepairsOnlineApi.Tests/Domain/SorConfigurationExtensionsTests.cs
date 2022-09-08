using System;
using System.Linq;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.Domain
{
    public class SorConfigurationExtensionsTests
    {
        [Theory]
        [MemberData(nameof(ValidEarlyExitSorConfigurations))]
        [MemberData(nameof(ValidSorConfigurationsWithEitherSorCodeOrOptions))]
        public void GivenValidSorConfiguration_WhenValidating_ThenReturnsTrue(SorConfiguration sorConfiguration)
        {
            // Act
            var actual = SorConfigurationExtensions.IsValid(sorConfiguration);

            // Assert
            Assert.True(actual);
        }

        public static TheoryData<SorConfiguration> ValidEarlyExitSorConfigurations()
        {
            var result = new TheoryData<SorConfiguration>();
            foreach (var earlyExitValue in EarlyExitValues.All)
            {
                result.Add(new SorConfiguration { Display = "display", Value = earlyExitValue });
            }

            return result;
        }

        public static TheoryData<SorConfiguration> ValidSorConfigurationsWithEitherSorCodeOrOptions() =>
            new()
            {
                new SorConfiguration { Display = "display", Value = "value", SorCode = "sor code" },
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    Options = new[]
                    {
                        new SorConfiguration { Display = "display", Value = "value", SorCode = "sor code" }
                    }
                },
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    Options = new[]
                    {
                        new SorConfiguration
                        {
                            Display = "display",
                            Value = "value",
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Display = "display", Value = "value", SorCode = "sor code"
                                }
                            }
                        }
                    }
                },
            };

        [Theory]
        [MemberData(nameof(InvalidSorConfigurationsWithBothSorCodeAndOptions))]
        [MemberData(nameof(InvalidSorConfigurationsWithInvalidOptions))]
        public void GivenInvalidSorConfiguration_WhenValidating_ThenReturnsFalse(SorConfiguration sorConfiguration)
        {
            // Act
            var actual = SorConfigurationExtensions.IsValid(sorConfiguration);

            // Assert
            Assert.False(actual);
        }

        public static TheoryData<SorConfiguration> InvalidSorConfigurationsWithBothSorCodeAndOptions() =>
            new()
            {
                new SorConfiguration { Display = "display", Value = "value" },
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    SorCode = "sor code",
                    Options = Array.Empty<SorConfiguration>()
                },
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    SorCode = "sor code",
                    Options = new[]
                    {
                        new SorConfiguration { Display = "display", Value = "value", SorCode = "sor code" }
                    }
                },
            };

        public static TheoryData<SorConfiguration> InvalidSorConfigurationsWithInvalidOptions() =>
            new()
            {
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    Options = Array.Empty<SorConfiguration>()
                },
                new SorConfiguration
                {
                    Display = "display",
                    Value = "value",
                    Options = new[]
                    {
                        new SorConfiguration
                        {
                            Display = "display",
                            Value = "value",
                            SorCode = "sor code",
                            Options = new[]
                            {
                                new SorConfiguration { Display = "display", Value = "value" }
                            }
                        }
                    }
                },
            };
    }
}
