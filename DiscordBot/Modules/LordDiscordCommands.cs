using Discord.Commands;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Modules
{
    public class LordDiscordCommands : ModuleBase<SocketCommandContext>
    {
        private LordDiscordServices _lordDiscordService;

        public LordDiscordCommands(LordDiscordServices lordDiscordService)
        {
            _lordDiscordService = lordDiscordService;
        }
    }
}
