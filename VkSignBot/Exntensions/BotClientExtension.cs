namespace VkSignBot.Exntensions
{
    public static class BotClientExtension
    {
        public static IServiceCollection AddBotClient(
            this IServiceCollection collection,
            Action<BotClientOptions> optionAction
        )
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (optionAction == null) throw new ArgumentNullException(nameof(optionAction));

            collection.Configure(optionAction);
            return collection.AddSingleton<IBotClient, BotClient>();
        }

        public static IServiceCollection AddBotClient(
            this IServiceCollection collection,
            IConfiguration config
        )
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (config == null) throw new ArgumentNullException(nameof(config));

            collection.Configure<BotClientOptions>(config);
            return collection.AddSingleton<IBotClient, BotClient>();
        }
    }
}