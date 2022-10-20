using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ardalis.GuardClauses;
using AutoMapper;
using HousingRepairsOnlineApi.Domain;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace HousingRepairsOnlineApi.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSoREngine(this IServiceCollection services, IRepairTypeSorConfigurationProvider sorConfigurationProvider)
        {
            Guard.Against.Null(sorConfigurationProvider, nameof(sorConfigurationProvider));

            var sorEngine = CreateSorEngine(sorConfigurationProvider);
            services.AddTransient<IDictionary<string, ISoREngine>>(_ =>
            {
                return new Dictionary<string, ISoREngine>
                {
                    { sorConfigurationProvider.RepairType, sorEngine }
                };
            });
            services.AddTransient<ISorEngineResolver, SorEngineResolver>();
        }

        public static ISoREngine CreateSorEngine(IRepairTypeSorConfigurationProvider sorConfigurationProvider)
        {
            var json = sorConfigurationProvider.ConfigurationValue();

            var sorConfigurations = ParseSorConfigurationJson(json).ToArray();

            if (sorConfigurations.Any(x => !x.IsValid()))
            {
                throw new InvalidOperationException(
                    "Invalid SOR configuration: each option should have a 'display' and 'value' value and either a SOR code and priority or additional options");
            }

            var journeyTriageOptions = GenerateJourneyRepairTriageOptions(sorConfigurations);
            var sorMapping = GenerateSorMapping(sorConfigurations);
            var result = new SoREngine(sorMapping, journeyTriageOptions);

            return result;
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

        public static IEnumerable<AppointmentSlotTimeSpan> ParseAppointmentSlotsConfigurationJson(string appointmentSlotsConfigurationValue)
        {
            var appointmentSlotTimeSpanSchema = GetJsonSchemaForType<AppointmentSlotTimeSpan>();

            var reader = new JsonTextReader(new StringReader(appointmentSlotsConfigurationValue));
            var validatingReader = new JSchemaValidatingReader(reader);
            validatingReader.Schema = appointmentSlotTimeSpanSchema;

            var serializer = new JsonSerializer();

            IEnumerable<AppointmentSlotTimeSpan> result;
            try
            {
                result = serializer.Deserialize<IEnumerable<AppointmentSlotTimeSpan>>(validatingReader);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Contents of appointment slots configuration value is malformed JSON.", e);
            }
            catch (JSchemaValidationException e)
            {
                throw new InvalidOperationException($"Contents of appointment slots configuration value doesn't match schema.", e);
            }

            return result;

            JSchema GetJsonSchemaForType<T>()
            {
                var type = typeof(T);
                var fullName = type.FullName;
                var manifestResourceStream = type.GetTypeInfo().Assembly
                    .GetManifestResourceStream(fullName + ".schema.json")!;
                var jsonSchema = new StreamReader(manifestResourceStream).ReadToEnd();
                var schema = JSchema.Parse(jsonSchema);
                return schema;
            }
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
                            ? CreateRepairTriageDetails(sorConfiguration2)
                            : sorConfiguration2.Options?.Where(c => !EarlyExitValues.All.Contains(c.Value)).Select(
                                sorConfiguration3 =>
                                {
                                    var key3 = sorConfiguration3.Value;
                                    var value3 = CreateRepairTriageDetails(sorConfiguration3);
                                    return new { key2 = key3, value2 = value3 };
                                }).ToDictionary(kvp => kvp.key2, kvp => kvp.value2);
                        return new { key = key2, value = value2 };
                    }
                ).ToArray();
                return new KeyValuePair<string, IDictionary<string, dynamic>>(key1, value1.ToDictionary(
                    kvp => kvp.key, kvp => kvp.value));
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return result;

            RepairTriageDetails CreateRepairTriageDetails(SorConfiguration sorConfiguration)
            {
                return new RepairTriageDetails { ScheduleOfRateCode = sorConfiguration.SorCode, Priority = sorConfiguration.Priority };
            }
        }
    }
}
