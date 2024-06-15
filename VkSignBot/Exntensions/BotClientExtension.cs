namespace VkSignBot.Exntensions {
    public static class BotClientExtension {
        public static IServiceCollection AddBotClient(
            this IServiceCollection collection,
            Action<BotClientOptions> optionAction
        ) {
            ArgumentNullException.ThrowIfNull(collection, nameof(collection));
            ArgumentNullException.ThrowIfNull(optionAction, nameof(optionAction));

            collection.Configure(optionAction);
            return collection.AddSingleton<IBotClient, BotClient>();
        }

        public static IServiceCollection AddBotClient(
            this IServiceCollection collection,
            IConfiguration config
        ) {
            ArgumentNullException.ThrowIfNull(collection, nameof(collection));
            ArgumentNullException.ThrowIfNull(config, nameof(config));

            collection.Configure<BotClientOptions>(config);
            return collection.AddSingleton<IBotClient, BotClient>();
        }
    }
}