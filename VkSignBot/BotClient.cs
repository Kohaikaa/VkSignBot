namespace VkSignBot
{
    public class BotClient : IBotClient
    {
        private readonly string? _botToken;
        private readonly string? _appToken;
        private readonly uint _appId;
        private readonly ulong _groupId;
        private IVkApi _userApi;
        private IVkApi _botApi;

        public IVkApi UserApi { get => _userApi; init => _userApi = value; }
        public IVkApi BotApi { get => _botApi; init => _botApi = value; }

        public BotClient(IOptions<BotClientOptions> options)
        {
            _botToken = options.Value.BotToken;
            _appToken = options.Value.AppToken;
            _appId = options.Value.AppId;
            _groupId = options.Value.GroupId;

            UserApi = new VkApi();
            BotApi = new VkApi();
            Authorize();
        }
        private void Authorize()
        {
            _userApi.Authorize(new ApiAuthParams
            {
                AccessToken = _appToken,
                ApplicationId = _appId
            });

            _botApi.Authorize(new ApiAuthParams
            {
                AccessToken = _botToken,
            });

            if ((_botApi.IsAuthorized && (_botApi.IsAuthorized || _userApi.IsAuthorized)) == false)
                throw new VkAuthorizationException("НBot is not authorized");
            Console.WriteLine("Bot is authorized");
        }
        public async Task StartPolling()
        {
            Console.WriteLine("Polling...");
            try
            {
                while (true)
                {
                    var longPollResponse = await _botApi!.Groups.GetLongPollServerAsync(_groupId);
                    var updates = await _botApi!.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                    {
                        Ts = longPollResponse.Ts,
                        Key = longPollResponse.Key,
                        Server = longPollResponse.Server,
                        Wait = 25
                    });

                    if (updates.Updates is null) continue;

                    Task.Run(async () =>
                    {
                        foreach (var update in updates.Updates)
                            await HandleUpdateAsync(update);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task HandleUpdateAsync(GroupUpdate update)
        {
            switch (update.Instance)
            {
                case MessageNew:
                    await HandleMessageAsync(update);
                    break;
            }
        }

        public async Task HandleMessageAsync(GroupUpdate update)
        {
            const char commandSymbol = '/';
            Message message = (update.Instance as MessageNew)!.Message;
            var text = message.Text.ToLower();

            if (!text.StartsWith(commandSymbol))
                return;

            var command = text.Split(new char[] { commandSymbol, ' ' })[1];
            var cmdService = new CommandService(this);
            switch (command)
            {
                case "роспись":
                    int startIndex = text.IndexOf(command);
                    if (message.Attachments.Count > 0)
                    {
                        var post = (message.Attachments.First(attach => attach.Type == typeof(Wall)).Instance as Wall);
                        await cmdService.MakeSign(message, post);
                        break;
                    }
                    var url = text.Substring(startIndex + command.Length + 1);
                    await cmdService.MakeSign(message, url);
                    break;
            }
        }
    }

    public class BotClientOptions
    {
        public string? BotToken { get; set; }
        public string? AppToken { get; set; }
        public uint AppId { get; set; }
        public ulong GroupId { get; set; }
    }
}