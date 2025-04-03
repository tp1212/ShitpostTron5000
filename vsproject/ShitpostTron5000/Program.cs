using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Markov;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ShitpostTron5000.CommandsModules;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.EntityFrameworkCore;


namespace ShitpostTron5000;


class Program
{
    public static DateTime Start;

    public static IConfigurationRoot Config;

    public static async Task Main()
    {
        Start = DateTime.Now;

        Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

        Log.Information("Initializing.");



        var client = new DiscordClient(new DiscordConfiguration
        {
            Token = Config["TokenString"],
            TokenType = TokenType.Bot
        });
        var interactivityExtension = client.UseInteractivity(new InteractivityConfiguration() { Timeout = TimeSpan.FromMinutes(5) });


        var services = new ServiceCollection()
            .AddTransient<ShitpostTronContext>(x => new ShitpostTronContext())
            .AddSingleton<Random>()
            .AddSingleton(interactivityExtension)
            .AddSingleton(client)
            .AddSingleton(new MarkovChain<string>(1))
            .BuildServiceProvider();

        await services.GetService<ShitpostTronContext>()
            .Database
            .MigrateAsync();

        var commands = client.UseSlashCommands(new SlashCommandsConfiguration()
        {
            Services = services,
        });
        
        commands.RegisterCommands<BasicCommands>();
        commands.RegisterCommands<Timers>();
        commands.RegisterCommands<QuoteDB>();
        commands.RegisterCommands<MarkovChain>();
        
        client.ClientErrored += async (sender, args) =>
        {
            Log.Logger.Error("Client Error", args.Exception);//Todo:use extra event data.
        };

        commands.SlashCommandExecuted += async (client, eventArgs) =>
             Log.Logger.Information($"Executed Command {eventArgs.Context.CommandName} for {eventArgs.Context.User}");

        commands.SlashCommandErrored += OnErrored;

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
        client.Ready += Client_Ready;
        await Task.Delay(-1);
    }

    private static async Task OnErrored(SlashCommandsExtension client, SlashCommandErrorEventArgs eventArgs)
    {
        var owner = await client.Client.Guilds[376781308845752340].GetMemberAsync(102061162195091456);
        await owner.SendMessageAsync(@$"{eventArgs.Context.CommandName}
had an error: 
```
{eventArgs.Exception}
```");

        Log.Logger.Error($"{eventArgs.Context.CommandName}:" + $"had an error {eventArgs.Exception.Message}:" + $" {eventArgs.Exception.Message}");

        await eventArgs.Context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"Whoops, that probably did not work, I got a {eventArgs.Exception}")
            .AsEphemeral());
    }

    private static async Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
    {
        //clear out old commands.

        foreach (var guild in sender.Guilds.Select(x => x.Key))
        {
            var cleanup = await sender.GetGuildApplicationCommandsAsync(guild);
            foreach (var command in cleanup)
            {
                await sender.DeleteGuildApplicationCommandAsync(guild, command.Id);
            }
        }
    }
}