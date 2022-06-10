using System;

namespace HousingRepairsOnlineApi.Helpers
{
    public static class EnvironmentVariableHelper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name) ??
                   throw new InvalidOperationException($"Incorrect configuration: '{name}' environment variable must be set");
        }
    }
}
