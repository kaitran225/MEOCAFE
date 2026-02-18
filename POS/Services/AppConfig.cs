using Microsoft.Extensions.Configuration;

namespace POS.Services
{
    public static class Config
    {
        public static IConfigurationRoot Configuration { get; private set; }

        static Config()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public static string? GetSetting(string key) => Configuration[key];
    }
}
