﻿using System;
using HousingRepairsOnlineApi.Domain;
using HousingRepairsOnlineApi.Helpers;
using Xunit;

namespace HousingRepairsOnlineApi.Tests.Domain
{
    public class SorConfigurationExtensionsTests
    {
        private const string DisplayValue = "display";
        private const string ValueValue = "value";
        private const string SorCodeValue = "sor code";

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
                result.Add(new SorConfiguration { Display = DisplayValue, Value = earlyExitValue });
            }

            return result;
        }

        public static TheoryData<SorConfiguration> ValidSorConfigurationsWithEitherSorCodeOrOptions() =>
            new()
            {
                new SorConfiguration { Display = DisplayValue, Value = ValueValue, SorCode = SorCodeValue },
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    Options = new[]
                    {
                        new SorConfiguration { Display = DisplayValue, Value = ValueValue, SorCode = SorCodeValue }
                    }
                },
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    Options = new[]
                    {
                        new SorConfiguration
                        {
                            Display = DisplayValue,
                            Value = ValueValue,
                            Options = new[]
                            {
                                new SorConfiguration
                                {
                                    Display = DisplayValue, Value = ValueValue, SorCode = SorCodeValue
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
                new SorConfiguration { Display = DisplayValue, Value = ValueValue },
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    SorCode = SorCodeValue,
                    Options = Array.Empty<SorConfiguration>()
                },
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    SorCode = SorCodeValue,
                    Options = new[]
                    {
                        new SorConfiguration { Display = DisplayValue, Value = ValueValue, SorCode = SorCodeValue }
                    }
                },
            };

        public static TheoryData<SorConfiguration> InvalidSorConfigurationsWithInvalidOptions() =>
            new()
            {
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    Options = Array.Empty<SorConfiguration>()
                },
                new SorConfiguration
                {
                    Display = DisplayValue,
                    Value = ValueValue,
                    Options = new[]
                    {
                        new SorConfiguration
                        {
                            Display = DisplayValue,
                            Value = ValueValue,
                            SorCode = SorCodeValue,
                            Options = new[]
                            {
                                new SorConfiguration { Display = DisplayValue, Value = ValueValue }
                            }
                        }
                    }
                },
            };
    }
}
