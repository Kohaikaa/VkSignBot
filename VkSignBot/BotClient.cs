using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VkSignBot
{
    public class BotClient : IBotClient
    {
        private static readonly Random Random = new Random();

        private readonly IVkApi _botApi;
        private readonly IVkApi _userApi;
        private readonly BotClientOptions _botClientOptions;

        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection;

        public IVkApi UserApi { get => _userApi; init => _userApi = value; }
        public IVkApi BotApi { get => _botApi; init => _botApi = value; }

        public BotClient(IOptions<BotClientOptions> options, IServiceCollection serviceCollection = null!)
        {
            _botClientOptions = options.Value;
            _botApi = new VkApi();
            _userApi = new VkApi();

            // Registration services
            _serviceCollection = serviceCollection ?? new ServiceCollection();
            _serviceCollection.AddTransient<CommandService>();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public async Task AuthorizeAsync()
        {
            if (_userApi.IsAuthorized && _botApi.IsAuthorized)
                return;

            await _userApi.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = _botClientOptions.AppToken,
                ApplicationId = _botClientOptions.AppId
            });

            await _botApi.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = _botClientOptions.BotToken,
            });

            if ((_botApi.IsAuthorized && (_botApi.IsAuthorized || _userApi.IsAuthorized)) == false)
                throw new VkAuthorizationException("Bot is not authorized");
            _serviceCollection.Replace(new ServiceDescriptor(typeof(IBotClient), this));
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            Console.WriteLine("Bot is authorized");
        }

        public async Task StartPolling()
        {
            Console.WriteLine("Polling...");
            try
            {
                while (true)
                {
                    var longPollResponse = await _botApi!.Groups.GetLongPollServerAsync(_botClientOptions.GroupId);
                    var updates = await _botApi!.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                    {
                        Ts = longPollResponse.Ts,
                        Key = longPollResponse.Key,
                        Server = longPollResponse.Server,
                        Wait = 25
                    });

                    if (updates.Updates is null) continue;
                    if (updates.Updates.Count == 0) continue;

                    var handleUpdatesTask = new Task(async () =>
                    {
                        foreach (var update in updates.Updates)
                            await HandleUpdateAsync(update);
                    });
                    handleUpdatesTask.Start();
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
            var text = message.Text.Trim().ToLower();

            if (!text.StartsWith(commandSymbol))
                return;

            var command = text.Split(new char[] { commandSymbol, ' ' })[1];
            var parameters =
                text.Split(' ').Length > 1 && text.IndexOf(command) == 1 ?
                text.Split(' ').AsSpan().Slice(1).ToArray() :
                null;
            
            var cmdService = _serviceProvider.GetRequiredService<CommandService>();
            switch (command)
            {
                case "роспись":
                    if (message.Attachments.Count > 0)
                    {
                        var post = (message.Attachments.First(attach => attach.Type == typeof(Wall)).Instance as Wall);
                        await cmdService.MakeSign(message, post);
                        break;
                    }
                    else if (parameters is null)
                    {
                        await _botApi.Messages.SendAsync(new MessagesSendParams
                        {
                            UserId = message.FromId,
                            RandomId = Random.NextInt64(),
                            Message = $"Ни ссылки на пост, ни репост поста я не вижу, а значит расписаться нигде не могу."
                        });
                        break;
                    }
                    await cmdService.MakeSign(message, parameters[0]);
                    break;
                default:
                    await _botApi.Messages.SendAsync(new MessagesSendParams
                    {
                        UserId = message.FromId,
                        RandomId = Random.NextInt64(),
                        Message = "Я не знаю такой команды, чел."
                    });
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