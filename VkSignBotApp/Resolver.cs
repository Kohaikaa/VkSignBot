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
                var cfg = context.Configuration;
                services.AddBotClient(cfg.GetSection("Vk"));
                services.AddSingleton<IBotClient, BotClient>();
            }).Build();
        }

        public static T Resolve<T>() where T : notnull => _host!.Services.GetRequiredService<T>();
    }
}