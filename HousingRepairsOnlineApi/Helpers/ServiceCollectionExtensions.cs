using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using AutoMapper;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HousingRepairsOnlineApi.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSoREngine(this IServiceCollection services, ISorConfigurationProvider sorConfigurationProvider)
        {
            Guard.Against.Null(sorConfigurationProvider, nameof(sorConfigurationProvider));

            var json = sorConfigurationProvider.ConfigurationValue();

            var sorConfigurations = ParseSorConfigurationJson(json).ToArray();

            if (sorConfigurations.Any(x => !x.IsValid()))
            {
                throw new InvalidOperationException(
                    "Invalid SOR configuration: each option should have a 'display' and 'value' value and either a SOR code or additional options");
            }

            var journeyTriageOptions = GenerateJourneyRepairTriageOptions(sorConfigurations);
            var sorMapping = GenerateSorMapping(sorConfigurations);
            services.AddTransient<ISoREngine, SoREngine>(_ => new SoREngine(sorMapping, journeyTriageOptions));
        }

        public static IEnumerable<SorConfiguration> ParseSorConfigurationJson(string sorConfigurationValueJson)
        {
            SorConfiguration[] result;
            try
            {
                result = JsonConvert.DeserializeObject<SorConfiguration[]>(sorConfigurationValueJson);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Contents of SOR configuration value is malformed JSON.", e);
            }

            return result;
        }

        public static IEnumerable<AppointmentSlotTimeSpan> ParseAppointmentSlotsConfigurationJson(string appointmentSlotsConfigurationValueJson)
        {
            AppointmentSlotTimeSpan[] result;
            try
            {
                result = JsonConvert.DeserializeObject<AppointmentSlotTimeSpan[]>(appointmentSlotsConfigurationValueJson);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Contents of appointment slots configuration value is malformed JSON.", e);
            }

            return result;
        }

        public static IEnumerable<RepairTriageOption> GenerateJourneyRepairTriageOptions(IEnumerable<SorConfiguration> sorConfigurations)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.CreateMap<SorConfiguration, RepairTriageOption>();
            });

            var mapper = mapperConfiguration.CreateMapper();
            var result =
                mapper.Map<IEnumerable<SorConfiguration>, IEnumerable<RepairTriageOption>>(sorConfigurations);

            return result;
        }

        public static IDictionary<string, IDictionary<string, dynamic>> GenerateSorMapping(
            IEnumerable<SorConfiguration> sorConfigurations)
        {
            var result = sorConfigurations.Select(sorConfiguration1 =>
            {
                var key1 = sorConfiguration1.Value;
                var value1 = sorConfiguration1.Options?.Where(c => !EarlyExitValues.All.Contains(c.Value)).Select(
                    sorConfiguration2 =>
                    {
                        var key2 = sorConfiguration2.Value;
                        dynamic value2 = !string.IsNullOrEmpty(sorConfiguration2.SorCode)
                            ? sorConfiguration2.SorCode
                            : sorConfiguration2.Options?.Where(c => !EarlyExitValues.All.Contains(c.Value)).Select(
                                sorConfiguration3 =>
                                {
                                    var key3 = sorConfiguration3.Value;
                                    var value3 = sorConfiguration3.SorCode;
                                    return new { key2 = key3, value2 = value3 };
                                }).ToDictionary(kvp => kvp.key2, kvp => kvp.value2);
                        return new { key = key2, value = value2 };
                    }
                ).ToArray();
                return new KeyValuePair<string, IDictionary<string, dynamic>>(key1, value1.ToDictionary(
                    kvp => kvp.key, kvp => kvp.value));
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return result;
        }
    }
}
