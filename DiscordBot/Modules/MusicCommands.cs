using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules.Services;
using Discord;

namespace DiscordBot.Modules
{
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        private MusicServices _musicService;

        public MusicCommands(MusicServices musicServices)   //Some MusicServices logic is done here instad of delegated to MusicServices,  not sure why i did that, will fix later.
        {
            _musicService = musicServices;
        }
        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            var perms = Context.Guild.CurrentUser.GetPermissions(user.VoiceChannel);
            if (user.VoiceChannel is null)
            {
                await ReplyAsync("You need to join the music voice chat");
                return;
            }
            else
            {
                if (perms.Connect == true)
                    await _musicService.ConnectAssync(user.VoiceChannel, Context.Channel as ITextChannel, user.Guild);
                else
                {
                    await ReplyAsync("I don't have the neccessary permissions to join your voice channel!");
                }
            }

        }

        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;
            await _musicService.DisconnectAssync(Context.Channel as ITextChannel);
        }
        [Command("Play")]  //empty play
        public async Task Play()
        {
            await Context.Channel.SendMessageAsync($"{Context.User.Username} please specify song");
        }
        [Command("QuickPlay")]
        public async Task QuickPlay([Remainder] string query)
        {
            if (!(Context.Guild.CurrentUser.VoiceChannel == null))
            {
                var result = _musicService.QuickPlayAsync(query, Context.Guild);
                await ReplyAsync(result.Result);
            }
            else
            {
                await ReplyAsync("I'm not in a voice chat");
            }
        }
        [Command("Play")]
        public async Task Play([Remainder]string query)  // searches and gives 5 options
        {
            if (!(Context.Guild.CurrentUser.VoiceChannel == null))
            {
                var result = _musicService.SearchAsync(query, Context.Guild, Context.Channel as ITextChannel);
            }
            else
            {
                await ReplyAsync("I'm not in a voice chat");
            }
        }
        [Command("Select")]
        public async Task Select([Remainder]int select) // selects one of the options
        {
            await _musicService.SelectAsync(select,Context.Channel as ITextChannel,Context.Guild);
        }

        [Command("Stop")]
        public async Task Stop()
        {
            await _musicService.StopAsync(Context.Channel as ITextChannel);
            await ReplyAsync("Stopped playing music");
        }

        [Command("Skip")]
        public async Task Skip()
        {
            var result = await _musicService.SkipAsync(Context.Channel as ITextChannel);
            await ReplyAsync(result);
        }
        [Command("Pause")]
        public async Task Pause()
        {
            EmbedBuilder embed;
            var result = await _musicService.PauseAsync(Context.Channel as ITextChannel);
            embed = Responses.CreateMessage(result);
            await ReplyAsync(embed: embed.Build());
        }
        [Command("Resume")]
        public async Task Resume()
        {
            EmbedBuilder embed;
            var result = await _musicService.ResumeAsync(Context.Channel as ITextChannel);
            embed = Responses.CreateMessage(result);
            await ReplyAsync(embed: embed.Build());
        }
        [Command("Volume")]
        public async Task Volume([Remainder]ushort vol)
        {
            if (vol > 150) vol = 150;
            if (vol < 1) vol = 1;
            await _musicService.SetVolumeAsync(vol, Context.Channel as ITextChannel);
            await ReplyAsync($"Set volume to {vol}");
        }

        [Command("OutputQueue")]
        public async Task OutputQueue()
        {
            await _musicService.OutputQueue(Context.Channel as ITextChannel);
        } 
    }
}
