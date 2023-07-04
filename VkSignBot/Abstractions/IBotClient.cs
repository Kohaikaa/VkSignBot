namespace VkSignBot.Abstractions
{
    public interface IBotClient
    {
        IVkApi UserApi { get; init; }
        IVkApi BotApi { get; init; }
        Task HandleMessageAsync(GroupUpdate update);
        Task HandleUpdateAsync(GroupUpdate update);
        Task StartPolling();
    }
}