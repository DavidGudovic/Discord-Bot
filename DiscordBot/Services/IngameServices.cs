using Discord;
using Discord.WebSocket;
using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace DiscordBot.Services
{
    public class IngameServices
    {
        public DiscordSocketClient _client;
        private Dictionary<string, Siege> _sieges;
        private Timer siegeTimer;
        public IngameServices(DiscordSocketClient client)
        {
            _client = client;
            _sieges = new Dictionary<string, Siege>();
            siegeTimer = new Timer(SiegeTimers,null,0, 6000);           
        }
        public async Task RemoveSiege(ITextChannel channel, string siege)   // removes the siege from the Disctionary
        {
            string message = "";
            if(_sieges.TryGetValue(siege,out Siege targetSiege))
            {
                await (_client.GetGuild(859396452873666590).GetChannel(881853053442076682) as ITextChannel).DeleteMessageAsync(targetSiege.CreationMessage);
                _sieges.Remove(siege);
                message = $"Removed {siege}";
            }
            else
            {
                message = "No such siege scheduled, check ``-when siege`` list for siege names";
            }
            EmbedBuilder embed = Responses.CreateMessage(message);
            await channel.SendMessageAsync(embed: embed.Build());
        }
        public async Task Time(ITextChannel target) //Displays the UTC time and time to Reset
        {          
            await target.SendMessageAsync($"Current server time is {DateTime.UtcNow.Hour}:{DateTime.UtcNow.Minute} \n{24 - DateTime.UtcNow.Hour - 1} hours {60 - DateTime.UtcNow.Minute} minutes to reset ");         
        }
        public async Task Siege(string siegeString, ITextChannel channel, IngameServices service)  // Creates the siege and adds it to our Dictionary
        {
            EmbedBuilder embed;
            bool passed = false;
            string[] siegeData = siegeString.Split(", ");
            DateTime time = DateTime.UtcNow;
            string location = "";
            string message;
            bool success = false;
            if (siegeData.Length == 2)   // if false - syntax error
            {
                location = siegeData[0].Trim();
                passed = DateTime.TryParse(siegeData[1], out time);
            }
            if (passed) // if false - syntax error
            {
                if (!_sieges.ContainsKey(location))  // If there is no siege set for that location
                {
                    TimeSpan remaining = time.Subtract(DateTime.UtcNow);
                    Console.WriteLine(remaining.ToString());
                    string zero = "";
                    if (time.Minute == 0) zero = "0"; // dumb af lol 
                    if (remaining.TotalMinutes > 0)  //if time makes sense
                    {
                        message = $"{channel.Guild.GetRole(862360736021217281).Mention}, {channel.Guild.GetRole(889014083888771112).Mention} sieging {location} {time.Month}/{time.Day} at {time.Hour}:{time.Minute}{zero} UTC\n**{remaining.Days} days {remaining.Hours} hours {remaining.Minutes} minutes from now!**";
                        _sieges.Add(location, new Siege(location, time));
                        success = true;
                    }
                    else
                    {
                        message = $"Cannot set a siege for a time in the past";
                    }
                }
                else
                {
                    message = $"There is already a siege set for {location} at {_sieges.GetValueOrDefault(location).Time.Hour}:{_sieges.GetValueOrDefault(location).Time.Hour} UTC";
                }
            }
            else
            {
               message = $"You've made a syntax error! Proper usage:\n``-siege [Location], [MM/dd/yyyy HH:MM]``";
            }

            embed = Responses.CreateMessage(message);
            if(!success || channel.Id != 881853053442076682)
            await channel.SendMessageAsync(embed: embed.Build());
            if(success)
            _sieges[location].CreationMessage = (await (_client.GetGuild(859396452873666590).GetChannel(881853053442076682) as ITextChannel).SendMessageAsync(embed: embed.Build())).Id; // posts in #marching-orders and stores the siege creation message
           //_sieges[location].CreationMessage = (await channel.SendMessageAsync(embed: embed.Build())).Id; // WHEN TESTING FOR NOT PINGING #MARCHING ORDERS
        }
        public async Task WhenSiege(ITextChannel channel, string siege) //Displays the UTC time and time to Reset
        {
            EmbedBuilder embed;
            string message = "";

            if (_sieges.Count == 0) message = "There are no sieges schedueled for today";

            if (siege.Equals("siege"))
            {
                foreach (Siege scheduledSiege in _sieges.Values)
                {
                    message += $"{scheduledSiege.Location} is scheduled for {scheduledSiege.Time.Month}/{scheduledSiege.Time.Day} at {scheduledSiege.Time.Hour}:{scheduledSiege.Time.Minute} UTC\n";
                }
            }
            else if (_sieges.TryGetValue(siege, out Siege scheduledSiege))
            {
                message += $"{scheduledSiege.Location} is set for {scheduledSiege.Time.Month}/{scheduledSiege.Time.Day} at {scheduledSiege.Time.Hour}:{scheduledSiege.Time.Minute} UTC";
            }
            else
            {
                message = "No such siege set, try using ``-when siege`` to all sieges for today";
            }
            embed = Responses.CreateMessage(message);
            await channel.SendMessageAsync(embed: embed.Build());     
        }
        public void SiegeTimers(Object Info) // Every 1 minute checks if any sieges are due, warns 1 hour prior, warns on start, removes Siege object 
        {
            string message = "";
            EmbedBuilder embed;
            string[] inspirationalQuotes;
            Console.WriteLine("Timer works      ----");
            
            foreach(Siege siege in _sieges.Values)
            {
                Console.WriteLine("Timer works      2222222222----");
                TimeSpan remaining = siege.Time.Subtract(DateTime.UtcNow);
                EditCountdown(siege, remaining);
                if((int)remaining.TotalMinutes == 60)
                {
                    message = $"{_client.GetGuild(859396452873666590).GetRole(862360736021217281).Mention}, {_client.GetGuild(859396452873666590).GetRole(889014083888771112).Mention}" +
                              $" 1 hour to {siege.Location} siege!\nGet ready";
                }
                else if(remaining.TotalMinutes <= 5)
                {
                    Random rd = new Random();
                    int rand = rd.Next(2);
                    inspirationalQuotes = new string[] { "Let there be carnage!", "Looks like meat's back on the menu boys!", "Chaaaargeeee!" };
                    message = $"{_client.GetGuild(859396452873666590).GetRole(862360736021217281).Mention}, {_client.GetGuild(859396452873666590).GetRole(889014083888771112).Mention}" +
                              $"{siege.Location} siege begins in {remaining.Minutes} minutes!\n{inspirationalQuotes[rand]}";
                    _sieges.Remove(siege.Location);
                }
                  embed = Responses.CreateMessage(message);
                  ((_client.GetGuild(859396452873666590).GetChannel(881853053442076682)) as ITextChannel).SendMessageAsync(embed: embed.Build()); // posts in #marching-orders
            }
        }
        public void EditCountdown(Siege siege,TimeSpan remaining)  // every 1 minute edits the countdown on the siege creation message 
        {
            string zero = "";
            string message = "";
            if (siege.Time.Minute == 0) zero = "0";
            if (remaining.Days > 0)
            {
                message = $"{_client.GetGuild(859396452873666590).GetRole(862360736021217281).Mention}," +
                    $" {_client.GetGuild(859396452873666590).GetRole(889014083888771112).Mention} sieging {siege.Location} " +
                    $"at {siege.Time.Month}/{siege.Time.Day}  {siege.Time.Hour}:{siege.Time.Minute}{zero} UTC\n**{remaining.Days} days {remaining.Hours} hours {remaining.Minutes} minutes from now!**";
            }
            else if (remaining.Hours > 0)
            {
                message = $"{_client.GetGuild(859396452873666590).GetRole(862360736021217281).Mention}," +
                $" {_client.GetGuild(859396452873666590).GetRole(889014083888771112).Mention} sieging {siege.Location} " +
                $"at {siege.Time.Month}/{siege.Time.Day}  {siege.Time.Hour}:{siege.Time.Minute}{zero} UTC\n**{remaining.Hours} hours {remaining.Minutes} minutes from now!**";
            }
            else if (remaining.Minutes > 0)
            {
                message = $"{_client.GetGuild(859396452873666590).GetRole(862360736021217281).Mention}," +
                $" {_client.GetGuild(859396452873666590).GetRole(889014083888771112).Mention} sieging {siege.Location} " +
                $"at {siege.Time.Month}/{siege.Time.Day}  {siege.Time.Hour}:{siege.Time.Minute}{zero} UTC\n**{remaining.Minutes} minutes from now!**";
            }
            else if (remaining.TotalMinutes <= 1)
            {
                message = $"{siege.Location}siege is over!";
                _sieges.Remove(siege.Location);
            }

            EmbedBuilder embed = Responses.CreateMessage(message);
            (_client.GetGuild(859396452873666590).GetChannel(881853053442076682) as ITextChannel).ModifyMessageAsync(siege.CreationMessage, msg => msg.Embed = embed.Build()); // #marching orders
            //  (_client.GetGuild(859396452873666590).GetChannel(901519429823791164) as ITextChannel).ModifyMessageAsync(siege.CreationMessage, msg => msg.Embed = embed.Build()); // When testing use this

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
