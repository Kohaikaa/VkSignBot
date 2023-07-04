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
            long postId = ParsePostId(postUrl);
            if (postId == -1)
                throw new ArgumentException("There's no such post id");

            await MakeSign(message.FromId, postId);
        }
        public async Task MakeSign(Message message, Wall wall)
        {
            if (message.FromId != wall.OwnerId)
            {
                await _botClient.BotApi.Messages.SendAsync(new MessagesSendParams
                {
                    UserId = message.FromId,
                    RandomId = _random.NextInt64(),
                    Message = $"Вижу, что это не ваша запись. Я оставлю только роспись под [id{message.FromId}|Вашим] постом на вашей стене."
                });
            }

            await MakeSign(message.FromId, (long)wall.Id!);
        }
        private async Task MakeSign(long? userId, long postId)
        {
            await _botClient.BotApi.Wall.CreateCommentAsync(new WallCreateCommentParams
            {
                PostId = postId,
                OwnerId = userId,
                Message = "Ты прекрасный репер"
            });
        }
        private long ParsePostId(string url)
        {
            // https://vk.com/wall685754570_20
            Int64.TryParse(url.Split('_')[1], out long postId);
            return postId >= 0 ? postId : -1;
        }
    }
}