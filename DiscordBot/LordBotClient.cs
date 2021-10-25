using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using DiscordBot.Modules.Services;
using DiscordBot.Services;
using Discord.Addons.Interactive;

namespace DiscordBot
{
    class LordBotClient  //sets up connection 
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;


        public LordBotClient(DiscordSocketClient client = null, CommandService cmdService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });

            _cmdService = cmdService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            }); 
          
        }

        public async Task RunAsync()  //Logins into the server and starts the service
        {
            await _client.LoginAsync(TokenType.Bot, "OTAxMjIwOTU4MzE1MTEwNDEx.YXMtbA.u_PkjuolbUUNQgjfjX9qjbAfRG8");  //TODO fetch token from .xaml/.json file instead of hardcode
            await _client.StartAsync();
            _client.Log += LogAsync;
            _services = SetupServices();

            var cmdHandler = new CommandHandler(_client, _cmdService, _services); 
            await cmdHandler.InitilizeAsync();

            await _services.GetRequiredService<MusicServices>().InitializeAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage logMessage) //Logs for console
        {
            Console.WriteLine(logMessage.Message + "\n"); ;
            return Task.CompletedTask;
        }

        private IServiceProvider SetupServices() => new ServiceCollection()  //Dependency injection
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton<LavaNode>()
            .AddSingleton<LavaConfig>()
            .AddSingleton<MusicServices>()
            .AddSingleton<ApplicationServices>()
            .AddSingleton<LordDiscordServices>()
            .AddSingleton<IngameServices>()
            .BuildServiceProvider();
    }
}
