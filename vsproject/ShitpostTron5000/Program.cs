using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using ShitpostTron5000.CommandsModules;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{

    public class StartUp
    {

    }

    class Program
    {
        public static DiscordClient Client;
        static CommandsNextModule _commands;
        public static DateTime Start;

        public static IConfigurationRoot Config;

        public static ShitpostTronContext GetDbContext()
        {
            return new ShitpostTronContext(new DbContextOptionsBuilder<ShitpostTronContext>()
                .UseSqlServer(Config["ShitpostTronDB"])
                .Options);
        }


        static void Main(string[] args)
        {
            Start = DateTime.Now;
            try
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Fatal("Main died",ex);
                throw;
            }
     

            while (true)
            {
                string msg = Console.ReadLine();
                Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), msg);
            }
        }

        static async Task Init(string[] args)
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.AzureTableStorage(
                    CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=shitposttronstorage;AccountKey=usqGBGKzJ0b8hAo3joCHNr2j1IfiWaTeFzkBBsPEjh/RCOuN+fRuAH+G/pIc/IvYwOK9CqZjyC1hF3GAs7BgFQ==;EndpointSuffix=core.windows.net")
                , storageTableName: "Complaints"
                    ).CreateLogger();

            Log.Information("Initializing.");



            await GetDbContext().Database.MigrateAsync();

  

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config["TokenString"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });
            Client.UseInteractivity(new InteractivityConfiguration() { Timeout = TimeSpan.FromMinutes(5) });
        }


        static async Task MainAsync(string[] args)
        {
            await Init(args);

            _commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            _commands.RegisterCommands<BasicCommands>();
            _commands.RegisterCommands<Timers>();
            _commands.RegisterCommands<QuoteDB>();

            Client.ClientErrored += async ex =>
            {
               Log.Logger.Error("Client Error",ex.Exception);

            };

            _commands.CommandExecuted += async eventArgs =>
                Log.Logger.Information($"Executed Command {eventArgs.Command} for {eventArgs.Context.User}");

            _commands.CommandErrored += async ex =>
            {
                Log.Logger.Error("Command Error", ex.Exception);
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


