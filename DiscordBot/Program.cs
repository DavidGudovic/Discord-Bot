using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
          => await new LordBotClient().RunAsync();
        public static void Close()
        {
            Environment.Exit(1);
        }
        
    }
}
