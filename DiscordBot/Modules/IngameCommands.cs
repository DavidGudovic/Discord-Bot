using Discord;
using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        [Command("join")] //common wrong spelling
        public async Task Join([Remainder] string lord)
        {           
            if(lord.Contains("lord"))
            {
                EmbedBuilder embed;
                embed = Responses.CreateMessage($"{Context.User.Username} you can join LORD by filling this survey: https://forms.gle/y9CPXhnH4mnuQi45A");
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            } 
        }
        [Command("stat")] //common wrong spelling
        public async Task Stat([Remainder] string unit)
        {
            await Stats(unit);
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
        [Command("siege")]
        public async Task Siege([Remainder]string siegeString)   //regex bullshit
        {
            if (Context.Guild.GetRole(881859016815444011).Members.Contains(Context.User))
            {
                await _ingameService.Siege(siegeString, Context.Channel as ITextChannel);
            }
            else
            {
                EmbedBuilder embed = Responses.CreateMessage($"You need to have the {Context.Guild.GetRole(881859016815444011).Mention} role to set sieges!");
                await ReplyAsync(embed: embed.Build());
            }
        }

    }
}
