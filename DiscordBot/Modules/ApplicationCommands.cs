using Discord;
using Discord.Commands;
using DiscordBot.Modules.Services;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class ApplicationCommands : ModuleBase<SocketCommandContext>
    {
        private ApplicationServices _appservice;
        public ApplicationCommands(ApplicationServices appservice)
        {
            _appservice = appservice;            
        }
        [Command("suggest")] // Mini docs for bot in chat
        public async Task Suggest([Remainder]string suggestion)
        {
            await _appservice.SuggestAsync(Context.User.Username,suggestion,Context.Channel as ITextChannel);
        }
        [Command("Help")] // Mini docs for bot in chat
        public async Task Help()
        {       
         await _appservice.Help(Context.Channel as ITextChannel);
        }
        [Command("Ping")] //Just checking the bot's alive
        public async Task Pong()
        {
            await _appservice.Pong(Context.Channel as ITextChannel);
        }
        [Command("Close")] //Shuts the bot down
        public async Task Close()   
        {
            if (Context.User.Username == "VolterVajt")  // Very bad security check, needs rework but is fine in context
                await _appservice.Close(Context.Channel as ITextChannel);
            else
                await ReplyAsync("Only Volt can use -close sry");
        }
    }
}
