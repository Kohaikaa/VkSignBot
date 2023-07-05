using System.Text.RegularExpressions;

namespace VkSignBot.Services
{
    public class CommandService
    {
        private static Random _random = new Random();
        private readonly IBotClient _botClient;

        public CommandService(IBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task MakeSign(Message message, string postUrl)
        {
            (long ownerId, long postId) = ParsePostUrl(postUrl);
            if (postId == 0)
            {
                await _botClient.BotApi.Messages.SendAsync(new MessagesSendParams
                {
                    UserId = message.FromId,
                    RandomId = _random.NextInt64(),
                    Message = $"Не удалось получить пост, возможно [id{message.FromId}|Ваша] ссылка не корректна."
                });
                return;
            }
            await MakeSign(message.FromId, postId, ownerId);
        }

        public async Task MakeSign(Message message, Wall? wall)
        {
            await MakeSign(message.FromId, wall!.Id, wall.OwnerId);
        }

        private async Task MakeSign(long? userId, long? postId, long? ownerId)
        {
            try
            {
                if (ownerId != userId)
                {
                    await _botClient.BotApi.Messages.SendAsync(new MessagesSendParams
                    {
                        UserId = userId,
                        RandomId = _random.NextInt64(),
                        Message =
                        "Вижу, что эта запись не на вашей странице. " +
                        $"Я оставлю только роспись под [id{userId}|Вашим] постом на Вашей странице."
                    });
                    return;
                }

                await _botClient.BotApi.Wall.CreateCommentAsync(new WallCreateCommentParams
                {
                    PostId = (long)postId!,
                    OwnerId = userId,
                    Message = "Ты прекрасный репер"
                });

                await _botClient.BotApi.Messages.SendAsync(new MessagesSendParams
                {
                    UserId = userId,
                    RandomId = _random.NextInt64(),
                    Message = $"Оставил тебе роспись, [id{userId}|милашка]"
                });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private (long, long) ParsePostUrl(string url)
        {
            string pattern = @"vk.com\/wall\d+_\d+";
            var matched = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
            if (!matched.Success)
                return (0, 0);

            pattern = @"\d+_\d+$";
            var ids = Regex.Match(url, pattern, RegexOptions.IgnoreCase).ToString();
            Int64.TryParse(ids.Split('_')[0], out long userId);
            Int64.TryParse(ids.Split('_')[1], out long postId);
            return (userId, postId);
        }
    }
}