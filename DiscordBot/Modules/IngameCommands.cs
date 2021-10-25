using Discord;
using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class IngameCommands : ModuleBase<SocketCommandContext>
    {
        private IngameServices _ingameService;

        public IngameCommands(IngameServices ingameService)
        {
            _ingameService = ingameService;
        }
        [Command("stats")]
        public async Task Stats()
        {
                await Context.Channel.SendMessageAsync($"{Context.User.Username} try using -stats list or -stats[unit]... i.e -stats crusher");
        }
        [Command("stats")]
        public async Task Stats([Remainder] string unit)
        {
                Console.WriteLine(unit);
                await _ingameService.Stats(unit.ToLower(), Context.Channel as ITextChannel);
        }
        [Command("time")]
        public async Task Time()
        {
            await _ingameService.Time(Context.Channel as ITextChannel);
        }
        
    }
}
