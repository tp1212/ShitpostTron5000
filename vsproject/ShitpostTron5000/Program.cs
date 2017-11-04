using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Blehgh
{
    class Program
    {
        public static DiscordClient Client;
        static CommandsNextModule _commands;
        public static DateTime Start;


        static void Main(string[] args)
        {
            Start = DateTime.Now;
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

            while (true)
            {
               string msg = Console.ReadLine();
                Client.SendMessageAsync( Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), msg);
            }
        }

        static async Task MainAsync(string[] args)
        {
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = "Mzc2NDE3OTE3ODc4Nzk2Mjg4.DN-PqA.nWQ6PTqdYzJpH14ZeUZ1s2_LGFI",
                TokenType = TokenType.Bot
            });

             _commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            _commands.RegisterCommands<BasicCommands>();

            Client.MessageCreated += async e =>
            {
                if(e.Channel.Name == "devtrons")
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };


            Client.VoiceStateUpdated += async e =>
            {
                await Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), $"{e.User.Username} just joined {e.Channel.Name}");
            };


            

            await Client.ConnectAsync();
           
        }
    }
}


