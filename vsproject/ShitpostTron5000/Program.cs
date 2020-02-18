using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;

namespace ShitpostTron5000
{
    class Program
    {
        public static DiscordClient Client;
        static CommandsNextModule _commands;
        public static DateTime Start;
        public static ShitpostTronContext ShitpostTronContext;


        static void Main(string[] args)
        {
            Start = DateTime.Now;
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

            while (true)
            {
                string msg = Console.ReadLine();
                Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), msg);
            }
        }

        static async Task init(string[] args)
        {


            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var Configuration = builder.Build();

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration["TokenString"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });

            ShitpostTronContext = new ShitpostTronContext();

        }


        static async Task MainAsync(string[] args)
        {

            await init(args);

            _commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            _commands.RegisterCommands<BasicCommands>();
            Client.ClientErrored += async e =>
            {

                Console.WriteLine(e.Exception.ToString());
                if (e.Exception.InnerException != null)
                {

                }
            };


            _commands.CommandErrored += async e =>
            {
                Console.WriteLine(e.Exception.ToString());
            };

            Client.MessageCreated += async e =>
            {
                if (e.Channel.Name == "devtrons")
                    if (e.Message.Content.ToLower().StartsWith("ping"))
                        await e.Message.RespondAsync("pong!");
            };


            Client.VoiceStateUpdated += async e =>
            {

                //await Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), $"{e.User.Username} just joined {e.Channel.Name}");
            };





            await Client.ConnectAsync();

        }
    }
}


