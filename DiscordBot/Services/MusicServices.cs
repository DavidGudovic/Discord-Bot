using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;
using System.Collections.Generic;

namespace DiscordBot.Modules.Services
{
    public class MusicServices  // Connects our services (Victoria/Lavalink)
    {
        private LavaNode _lavaNode;
        public DiscordSocketClient _client;
        private LavaPlayer _player;

        private Dictionary<ITextChannel, IEnumerable<LavaTrack>> _selection; // Saves the search results in context to where they were searched from
        private Dictionary<ITextChannel, bool> _isSelecting;
        private Dictionary<ITextChannel, ulong> _lastMessageID;


        public MusicServices(LavaNode lavaNode, DiscordSocketClient client)
        {
            _client = client;
            _lavaNode = lavaNode;
            _selection = new Dictionary<ITextChannel, IEnumerable<LavaTrack>>();
            _isSelecting = new Dictionary<ITextChannel, bool>();
            _lastMessageID = new Dictionary<ITextChannel, ulong>();
        }

        public Task InitializeAsync()  //self explanatory 
        {
            _client.Ready += clientReadyAsync;
            _lavaNode.OnLog += LogEvent;
            _lavaNode.OnTrackEnded += TrackFinished; ;
            return Task.CompletedTask;
        }
        public async Task ConnectAssync(SocketVoiceChannel channel, ITextChannel textChannel, IGuild guild) //Connects to voicechat
        {
            if (_lavaNode.HasPlayer(guild))
            {
                _player = _lavaNode.GetPlayer(guild);
                await textChannel.SendMessageAsync($"Already joined {_player.VoiceChannel.Name} try using -leave");
            }
            else 
            {
                await _lavaNode.JoinAsync(channel, textChannel);
                _player = _lavaNode.GetPlayer(guild); 
                await textChannel.SendMessageAsync($"Sucesfully connected to {channel.Name}");
            }
        }
        public async Task DisconnectAssync(ITextChannel text)  // leaves the voice channel
        {
            _player = _lavaNode.GetPlayer(text.Guild);
            IVoiceChannel currentChannel = _player.VoiceChannel;
            if (!(currentChannel is null))
            {
                if (_player.PlayerState == PlayerState.Playing) await StopAsync(text); 
                await _lavaNode.LeaveAsync(currentChannel);
                await text.SendMessageAsync($"Left {currentChannel.Name}");
            } else
            {
                await text.SendMessageAsync("I'm not in any voice channels currently");
            }
        }
        public async Task<string> QuickPlayAsync(string query, IGuild guild) // Searches for a track on youtube and plays the first result
        {
            _player = _lavaNode.GetPlayer(guild);
            var results = await _lavaNode.SearchYouTubeAsync(query);     //searches
            if (results.Status == SearchStatus.LoadFailed || results.Status == SearchStatus.NoMatches)
            {
                return "No matches found";
            }
            var track = results.Tracks.FirstOrDefault();    //stores result

            if (_player.PlayerState == PlayerState.Playing)    //plays or queues it
            {
                _player.Queue.Enqueue(track);
                return $"{track.Title} has been added to the queue";
            }
            else
            {
                await _player.PlayAsync(track);
                return $"Now playing {track.Title}";
            }
        }
        public async Task<string> SearchAsync(string query, IGuild guild, ITextChannel textChannel) // Searches for a track on youtube, gives 5 options, sets isSelecting to true
        {
            _player = _lavaNode.GetPlayer(guild);
            EmbedBuilder embed;
            var results = await _lavaNode.SearchYouTubeAsync(query);     //searches
            if(results.Status == SearchStatus.LoadFailed || results.Status == SearchStatus.NoMatches)
            {
                return ("Search failed");
            }
            string selection = "";
            int inum = 1; 
            foreach(LavaTrack track in results.Tracks.Take(10))
            {
                selection += $"{inum}. {track.Title}\n";
                inum += 1;
            }
            embed = Responses.CreateMessage($"**Type -select # to select a track**\n{selection}");
            var msg = await textChannel.SendMessageAsync(embed: embed.Build());
            if (!_isSelecting.ContainsKey(textChannel)) 
            { 
            _lastMessageID.Add(textChannel, msg.Id); //testing
            _isSelecting.Add(textChannel, true);  //testing 
            _selection.Add(textChannel, results.Tracks.Take(10));// testing
            }
            else
            {

                _isSelecting.Remove(textChannel);   // removes the selection, isSelecting and relevant MessageId in current context
                _selection.Remove(textChannel);
                _lastMessageID.Remove(textChannel);

                _lastMessageID.Add(textChannel, msg.Id); //testing
                _isSelecting.Add(textChannel, true);  //testing 
                _selection.Add(textChannel, results.Tracks.Take(10));// testing
            }
            return ($"{selection}");
        }
        public async Task SelectAsync(int select, ITextChannel textChannel, IGuild guild)
        {
            _player = _lavaNode.GetPlayer(guild);
            bool selecting;
            _isSelecting.TryGetValue(textChannel, out selecting);   // Gets if selection is on for the current context
            if (!selecting)
            {
                await textChannel.SendMessageAsync("You haven't been given a selection");
            }
            else
            {
               
                _selection.TryGetValue(textChannel, out IEnumerable<LavaTrack> tracks);  // gets the selection for current context 
                _lastMessageID.TryGetValue(textChannel, out ulong lastMsgId); //gets relevant in context bot message
                var track = tracks.ElementAt(select - 1);


                _isSelecting.Remove(textChannel);   // removes the selection, isSelecting and relevant MessageId in current context
                _selection.Remove(textChannel);
                _lastMessageID.Remove(textChannel); 
                EmbedBuilder embed;

                if (_player.PlayerState == PlayerState.Playing)    //plays or queues it
                {
                    _player.Queue.Enqueue(track);
                    
                    embed = Responses.CreateMessage($"Added {track.Title} to queue");
                    await textChannel.ModifyMessageAsync(lastMsgId, msg => msg.Embed = embed.Build()); // Magic really

                }
                else
                {
                    await _player.PlayAsync(track);
                    embed = Responses.CreateMessage($"Now playing {track.Title}");
                    await textChannel.ModifyMessageAsync(lastMsgId, msg => msg.Embed = embed.Build());
                }
            }
        }

        public async Task StopAsync(ITextChannel channel) //Stops playing the song
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
            if (_player is null) return; 
            else
            await _player.StopAsync();
        }
        public async Task<string> PauseAsync(ITextChannel channel)
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
            if (_player.PlayerState == PlayerState.Playing)
            {
                await _player.PauseAsync();              
                return "paused";
            }
            else
            {
                return "Not playing anything";
            }
        }
        public async Task<string> ResumeAsync(ITextChannel channel)
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
            if (_player.PlayerState != PlayerState.Playing)
            {
                await _player.ResumeAsync();
                return "resumed";
            }
            else
            {
                return "Already playing";
            }
        }
        public async Task<string> SkipAsync(ITextChannel channel)
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
            await _player.SkipAsync();
            return $"Skipping current track.\n Now playing {_player.Track.Title}";
        } // skips the current song  TODO: Make skip selectable
        public async Task SetVolumeAsync(ushort vol,ITextChannel channel) // sets volume
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
             if (_player is null) return;        
                await _player.UpdateVolumeAsync(vol);
        }
        public async Task OutputQueue(ITextChannel channel) // outputs the # of elements and their titles Note: Possibly will remove later
        {
            _player = _lavaNode.GetPlayer(channel.Guild);
            string result = $"{_player.Queue.Count} elements in queue: ";
            foreach(LavaTrack track in _player.Queue)
            {
                result += $"\n{track.Title}";
            }
            await _player.TextChannel.SendMessageAsync(result);
        }
        private async Task TrackFinished(TrackEndedEventArgs trackInfo) //checks if queue is empty, if not, plays next

        {
            _player = _lavaNode.GetPlayer(trackInfo.Player.TextChannel.Guild);
            if (_player.Queue.Count == 0)
            {
                await trackInfo.Player.TextChannel.SendMessageAsync("No more tracks in queue");
            }
            else if(trackInfo.Reason == TrackEndReason.Finished)
            {
                await _player.PlayAsync(_player.Queue.First());
                _player.Queue.TryDequeue(out LavaTrack current);
                await _player.TextChannel.SendMessageAsync($"Now playing {current.Title} from queue");
            }
            
        }
        private Task LogEvent(LogMessage msg) // logs stuff eh
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
        private async Task clientReadyAsync() // Once the connection's ready connect the Lava node
        {
          if (!_lavaNode.IsConnected) { await _lavaNode.ConnectAsync(); }
        }

     
    }
}
