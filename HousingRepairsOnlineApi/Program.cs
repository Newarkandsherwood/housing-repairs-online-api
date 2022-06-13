using System;
using HousingRepairsOnlineApi.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HousingRepairsOnlineApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // webBuilder.UseSentry(o =>
                    // {
                    //     o.Dsn = EnvironmentVariableHelper.GetEnvironmentVariable("SENTRY_DNS");
                    //
                    //     var environment = EnvironmentVariableHelper.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    //     if (environment == Environments.Development)
                    //     {
                    //         o.Debug = true;
                    //         o.TracesSampleRate = 1.0;
                    //     }
                    // });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
