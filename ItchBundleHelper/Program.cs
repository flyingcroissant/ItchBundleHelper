using Microsoft.Extensions.Configuration;

namespace ItchBundleHelper
{
    internal class Program
    {
        static Settings _appSettings = new();

        static async Task Main()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = configBuilder.Build();

            ConfigurationBinder.Bind(configuration.GetSection(nameof(Settings)), _appSettings);

            Helper app = new(_appSettings);
            await app.Run();
        }
    }
}