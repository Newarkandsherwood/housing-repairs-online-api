using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
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

            IDictionary<string, IDictionary<string, dynamic>> soRMapping;
            try
            {
                soRMapping = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, dynamic>>>(json);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Contents of SOR configuration value is malformed JSON.", e);
            }

            services.AddTransient<ISoREngine, SoREngine>(_ => new SoREngine(soRMapping));
        }
    }
}
