using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class IngameServices
    {
        public DiscordSocketClient _client;
        public IngameServices(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task Time(ITextChannel target) //Displays the UTC time and time to Reset
        {          
            await target.SendMessageAsync($"Current server time is {DateTime.UtcNow.Hour}:{DateTime.UtcNow.Minute} \n{24 - DateTime.UtcNow.Hour - 1} hours {60 - DateTime.UtcNow.Minute} minutes to reset ");         
        }
        public async Task Siege(string siegeString, ITextChannel channel)  // -siege Adhelrond 22:00  
        {
            EmbedBuilder embed;
            string[] siegeData = siegeString.Split(',', ':');
            string location = siegeData[0].Trim();
            int hour = Int32.Parse(siegeData[1].Trim());
            int minute = Int32.Parse(siegeData[2].Trim());
            int hourUntil = hour - DateTime.UtcNow.Hour - 1;
            int minuteUntil = (60 - DateTime.UtcNow.Minute) + minute;
            if(minuteUntil > 60)
            {
                hourUntil += 1;
                minuteUntil -= 60;
            }
            string zero = "";
            if (minute == 0) zero = "0"; // dumb af lol 
            embed = Responses.CreateMessage($"{channel.Guild.GetRole(862360736021217281).Mention}, {channel.Guild.GetRole(889014083888771112).Mention} sieging {location} at {hour}:{minute}{zero} UTC\n**{hourUntil} hours {Math.Abs(minuteUntil)} minutes from now!**");
            await channel.SendMessageAsync(embed: embed.Build());
            
        }

        public async Task Stats(string unit, ITextChannel textChannel)  // posts the image of required stats from Images/Stats in project folder or lists the available.
        {
            string folderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\", @"Images\stats"));
            string fullPath = Path.Combine(folderPath, unit + ".PNG");
            if (unit != "list")
            {
                try
                {
                    await textChannel.SendFileAsync(fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await textChannel.SendMessageAsync("I don't recognize the unit, try using -stats list");
                }
            } else
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                FileInfo[] files = di.GetFiles("*.PNG");
                string allUnits = "";
                foreach (FileInfo file in files)
                {
                    string temp = file.Name.ToString().Substring(0,file.Name.Length - 4);
                    allUnits += " " + temp + ",";
                }
                await textChannel.SendMessageAsync($"Currently available units are:\n{allUnits.Substring(1,allUnits.Length-2)}");
            }
        }
    }
}
