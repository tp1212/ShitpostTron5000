using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Markov;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using ShitpostTron5000.CommandsModules;

namespace ShitpostTron5000
{

    public class StartUp
    {

    }

    class Program
    {
        public static DateTime Start;

        public static IConfigurationRoot Config;

        public static async Task Main()
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

          

            var client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config["TokenString"],
                TokenType = TokenType.Bot
            });
            var interactivityExtension = client.UseInteractivity(new InteractivityConfiguration() { Timeout = TimeSpan.FromMinutes(5) });


            var services = new ServiceCollection()
                .AddTransient<ShitpostTronContext>(x=>new ShitpostTronContext(new DbContextOptionsBuilder<ShitpostTronContext>()
                    .UseSqlServer(Config["ShitpostTronDB"])
                    .Options))
                .AddSingleton<Random>()
                .AddSingleton(interactivityExtension)
                .AddSingleton(client)
                .AddSingleton(new MarkovChain<string>(1))
                .BuildServiceProvider();

            await services.GetService<ShitpostTronContext>()!
                .Database
                .MigrateAsync();

            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new []{"!"},
                CaseSensitive = false,
                Services = services,
            });
            commands.RegisterCommands<BasicCommands>();
            commands.RegisterCommands<Timers>();
            commands.RegisterCommands<QuoteDB>();
            commands.RegisterCommands<MarkovChain>();

            client.ClientErrored += async (sender, args ) =>
            {
               Log.Logger.Error("Client Error",args.Exception);//Todo:use extra event data.
            };

            commands.CommandExecuted += async (client,eventArgs) =>
                Log.Logger.Information($"Executed Command {eventArgs.Command} for {eventArgs.Context.User}");

            commands.CommandErrored += async (client, eventArgs) =>
            {
                var owner = await client.Client.Guilds[376781308845752340].GetMemberAsync(102061162195091456);
                
                await owner.SendMessageAsync(@$"{eventArgs.Command}
had an error: 
```
{eventArgs.Exception}
```");

                Log.Logger.Error($"{eventArgs.Command}:" +
                                 $"had an error:" +
                                 $" {eventArgs.Exception}");
            };

            client.MessageCreated += async (sender, e) =>
            {
                if (e.Channel.Name == "devtrons")
                    if (e.Message.Content.ToLower().StartsWith("ping"))
                        await e.Message.RespondAsync("pong!");
            };

            //Client.VoiceStateUpdated += async e =>
            //{
            //    //await Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), $"{e.User.Username} just joined {e.Channel.Name}");
            //};

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}


