using Microsoft.Extensions.Configuration;

namespace VkSignBotApp
{
    public static class Resolver
    {
        private static IHost? _host;

        public static void BuildServices()
        {
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                IConfiguration cfg = configBuilder.Build();
                services.AddBotClient(cfg.GetSection("Vk"));
                services.AddSingleton(typeof(IServiceCollection), services);
            }).Build();
        }

        public static T Resolve<T>() where T : notnull => _host!.Services.GetRequiredService<T>();
    }
}