using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using EmbedThumbnail = DiscordBot.Embedding.EmbedThumbnail;


namespace DiscordBot
{
    public class Responses
    {
        public static EmbedBuilder Create(IUser user)
        {
            return new EmbedBuilder().WithAuthor(user.Username, user.GetAvatarUrl())
                                     .WithColor(Color.Teal)
                                     .WithFooter("Post made at ")
                                     .WithCurrentTimestamp();
        }

        public static EmbedBuilder CreateSuccess(IUser user, string successMessage)
        {
            return new EmbedBuilder().WithAuthor(user.Username, user.GetAvatarUrl())
                                     .WithColor(Color.Red)
                                     .WithTitle("Error")
                                     .WithDescription($"```diff\n+ {successMessage}\n```")
                                     .WithThumbnailUrl(EmbedThumbnail.Success)
                                     .WithFooter("Post made at ")
                                     .WithCurrentTimestamp();
        }

        public static EmbedBuilder CreateError(IUser user, string errorMessage)
        {
            return new EmbedBuilder().WithAuthor(user.Username, user.GetAvatarUrl())
                                     .WithColor(Color.Red)
                                     .WithTitle("Error")
                                     .WithDescription("Something went wrong, see detailed error message below")
                                     .AddField("Message", $"```diff\n- {errorMessage}\n```")
                                     .WithThumbnailUrl(EmbedThumbnail.Error)
                                     .WithFooter("Post made at ")
                                     .WithCurrentTimestamp();
        }

        public static EmbedBuilder CreateMessage(string message)
        {
            return new EmbedBuilder().WithDescription(message)
                                     .WithColor(Color.Teal);
        }
        public static EmbedBuilder CreateSiegeMessage(string comment,string url,string location, string time)   // for testing
        {
            return new EmbedBuilder().WithTitle("**SIEGE NOTICE**")
                                     .WithDescription(comment)
                                     .WithImageUrl(url)
                                     .WithFields(new EmbedFieldBuilder().WithIsInline(true).WithName("Where").WithValue(location), new EmbedFieldBuilder().WithIsInline(true).WithName("When").WithValue($"{time} UTC"))
                                     .WithColor(Color.Teal);
        }
    }
}