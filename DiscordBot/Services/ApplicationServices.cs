using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Modules.Services
{
    public class ApplicationServices
    {
        public DiscordSocketClient _client;
        public ApplicationServices(DiscordSocketClient client)
        {
            _client = client;
        }
        public async Task SuggestAsync(string username, string suggestion, ITextChannel channel)
        {
            EmbedBuilder embed;
            string filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\", @"logs\suggestions.txt"));
            embed = Responses.CreateMessage($"Appreciate the suggestion {username}!");
            await channel.SendMessageAsync(embed: embed.Build());
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"{username} suggests:\n{suggestion}   at {DateTime.Now}\n");
            }
        }
        public async Task Help(ITextChannel channel) //writes out mini docs in command source channel
        {                    
            EmbedBuilder embed;
            string helpMessage = 
                $"**Game related**\n" +
                $"``-time``   Displays current server time and time to server reset\n" +
                $"``-siege [keep], [Time in UTC]`` Sets the siege time and place, alerts LORD members\n" +
                $"``-when [siege]``  Displays when a siege is scheduled\n" +
                $"``-removeSiege [location]``   Unschedules the siege\n" +
                $"``-join lord``   Displays the link to join LORD\n" +
                $"``-stats [unit]``    Displays a picture of the units stats\n" +
                $"``-stats list``   Displays a list of available units\n\n" +
                $"**Music related**\n" +
                $"``-join``   Joins the voice chat you are in\n" +
                $"``-leave``   Leaves the voice chat you are in\n" +
                $"``-play [song]``    Searches for the song and lets you pick from 10 search results\n" +
                $"``-select #``   If you searched for a song selects one from the list provided\n" +
                $"``-quickPlay [song]``   Plays the song or adds it to queue\n" +
                $"``-skip``   Skips current song\n" +
                $"``-stop``   Stops playing music\n" +
                $"``-pause``   Pauses playing music\n" +
                $"``-resume``   Stops playing music\n" +
                $"``-volume [0-150]``   Sets the volume\n" +
                $"``-outputqueue``   Displays the current queue\n\n" +
                $"**Bot related**\n" +
                $"``-suggest [text]``  Send Volt your suggestion, it'll probably be made\n" +
                $"``-ping``   Replies pong...\n" +
                $"``-close``   If you are authorized, shuts the bot down\n" +
                $"``-help``   Displays this help info\n";
            embed = Responses.CreateMessage(helpMessage);
            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task Close(ITextChannel channel)  // shuts the app down
        {
            await channel.SendMessageAsync($"Shutting down");
            Program.Close();
        }
        public async Task Pong(ITextChannel channel) // outputs "Pong"
        {
            await channel.SendMessageAsync("Pong!");
        }

    }
}
