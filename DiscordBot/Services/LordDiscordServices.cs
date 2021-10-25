using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Services
{
    public class LordDiscordServices
    {
        public DiscordSocketClient _client;
        public LordDiscordServices(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
