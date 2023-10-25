namespace VkSignBotApp;
public class Program
{
    private static async Task Main(string[] args)
    {
        Resolver.BuildServices();
        var bot = Resolver.Resolve<IBotClient>();
        await bot.AuthorizeAsync();
        await bot.StartPolling();
    }
}