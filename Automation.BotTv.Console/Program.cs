using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium.BiDi;
using Automation.BotTv.Console.Helper;
using Automation.BotTv.Repository;

namespace Automation.BotTv.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.SetBasePath(Directory.GetCurrentDirectory());
                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configuration.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMemoryCache();
                    services.AddTransient<TokenService>();
                    services.AddTransient<TokenHttpHandler>();
                    services.AddHttpClient("BotTvHttpClient", client =>
                    {
                        var configuration = hostContext.Configuration;
                        client.BaseAddress = new Uri(configuration.GetSection("BaseUrl").Value);
                    })
                    .AddHttpMessageHandler<TokenHttpHandler>();

                    services.AddTransient<ApiService>();
                    services.AddSingleton<AutomationService>();
                    services.AddHostedService<Worker>();
                });
    }
}