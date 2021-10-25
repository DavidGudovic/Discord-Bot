using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot
{
    public class CommandHandler //handles.. well.. commands
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services; 

        public CommandHandler(DiscordSocketClient client, CommandService cmdService,IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        }

        public async Task InitilizeAsync() // runs and adds the services
        {
            await _cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), _services); //adding modules and injecting dependancies
            _cmdService.Log += LogAsync;
            _client.MessageReceived += HandleMessageAsync; 
        }

        private async Task HandleMessageAsync(SocketMessage msg) //handles all messages the bot sees
        {
           // Console.Write(msg.Content);
            var argPos = 0;
            if (msg.Author.IsBot) return;

            var userMessage = msg as SocketUserMessage;
            if (userMessage is null) return;

            if (!userMessage.HasCharPrefix('-', ref argPos))   // defines the prefix for commands
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _cmdService.ExecuteAsync(context, argPos, _services);
        }

        private Task LogAsync(LogMessage logMessage) // logs stuff... 
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
