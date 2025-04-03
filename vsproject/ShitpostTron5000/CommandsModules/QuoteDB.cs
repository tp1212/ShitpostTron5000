using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Serilog;
using ShitpostTron5000.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShitpostTron5000.CommandsModules;

[SlashModuleLifespan(SlashModuleLifespan.Transient)]
public class QuoteDB : ApplicationCommandModule
{
    private readonly ShitpostTronContext _db;
    private readonly InteractivityExtension _interactivity;
    private readonly DiscordClient _client;

    public QuoteDB(ShitpostTronContext db, InteractivityExtension interactivity, DiscordClient client)
    {
        _db = db;
        _interactivity = interactivity;
        _client = client;
    }


    [SlashCommand("QuoteRandom", "Get a quote at random, yay.")]
    public async Task GetRandomQuote(InteractionContext ctx)
    {
        if (!_db.Quotes.Any())
            return;


        var quoteNumber = new Random().Next(1,_db.Quotes.Count()+1);
        await SayQuote(ctx, quoteNumber);
    }

    [SlashCommand("Quote", "Get a quote by number, yay.")]
    public async Task GetQuote(InteractionContext ctx, [Option("number", "...")] long number)
    {
        if (!_db.Quotes.Any())
            return;

        await SayQuote(ctx, number);
    }

    [SlashCommand("QuoteBrowser", "Get a browser for quotes. yay.")]
    public async Task GetPaginatedQuote(InteractionContext ctx)
    {
        await ctx.DeferAsync(true);
        var inter = _client.GetInteractivity();
        var pages = _db.Quotes.Select(x => new Page(QuoteToString(x), null));
        await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, PaginationBehaviour.Ignore);
    }

    private async Task SayQuote(InteractionContext ctx, double quoteNumber)
    {
        var result = _db.Quotes.FirstOrDefault(x => x.Id == quoteNumber);
        if (result == null)
        {
            await ctx.CreateResponseAsync($"Could not find quote #{quoteNumber}");
        }

        await ctx.CreateResponseAsync(QuoteToString(result));
    }

    private static string QuoteToString(Quote result)
    {
        return $"{result.QuoteText.Replace("\n", "\n> ")} \n―{result.QuoteeName} \t#{result.Id}";
    }


    [SlashCommand("QuoteManual", "Manually add a quote, like for when you hear something\n eg !quoteManual @Steve Funny words go here.")]
    public async Task QuoteFromUser(InteractionContext ctx, [Option("user", "The user to attribute this to.")] DiscordUser attributeToMember, [Option("text", "the stuff to put in the quote")] string Content)
    {
        var attributeToName = (await ctx.Guild.GetMemberAsync(attributeToMember.Id)).DisplayName;
        await ctx.DeferAsync();

        var mention = new Regex("<@!(?<id>\\d+)>");

        Content = mention.Replace(Content, MentionTron);
        var response = await ctx.GetOriginalResponseAsync();
        await response.ModifyAsync($"Want me to add this to the quote DB?\n> {Content.Replace("\n", "\n> ")} \n―{attributeToName}");


        await response.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
        await response.CreateReactionAsync(DiscordEmoji.FromUnicode("❎"));

        var choice = await _interactivity.WaitForReactionAsync(x => x.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase)
                                                                    || x.Emoji.Name.Contains("❎", StringComparison.OrdinalIgnoreCase),
            ctx.User);

        try
        {
            await response.DeleteAllReactionsAsync();
        }
        catch (Exception ex)
        {
            Log.Warning("did not clean reactons", ex);
        }

        if (choice.Result.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase))
        {
            await response.ModifyAsync("Working.");


            Quote q = new Quote
            {
                DiscordSnowFlake = 0,
                QuoteText = Content,
                QuoteeName = attributeToName,
                QouteeDiscordSnowflake = attributeToMember.Id,

            };
            _db.Add(q);
            await _db.SaveChangesAsync();
            await response.ModifyAsync("Done!");

        }
        else
        {
            await response.ModifyAsync("No good after all?");
        }
        await Task.Delay(TimeSpan.FromSeconds(6));
        await response.DeleteAsync();
    }



    [SlashCommand("QuoteChat", "quote someone directly from the channel you are typing this in.")]
    public async Task QuoteChat(InteractionContext ctx,
        [Option("quotee", "the person you are quoting")]
        DiscordUser qotee,
        [Option("skip", "The amount of the qotee's messages to skip over.")]
        double skip = 0)
    {
        var candidates = await ctx.Channel.GetMessagesAsync(50);
        var targetMsg = candidates.Where(x => x.Author == qotee)
            .Skip((int)skip)
            .FirstOrDefault();
        
        if (targetMsg == null)
        {
            await ctx.CreateResponseAsync($"I cant find a recent message (I check the last 50 of the channel) by that user{(skip != 0 ? $" or I skipped it." : "")}.");
            return;
        }

        var inter = _client.GetInteractivity();

        var bleh = new DiscordActionRowComponent(new DiscordComponent[]
        {
            new DiscordButtonComponent(ButtonStyle.Primary, "yes", "yes",
                emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
            new DiscordButtonComponent(ButtonStyle.Primary, "no", "no",
                emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("❎"))),
        });


        var interactionResponse = new DiscordInteractionResponseBuilder()
            .WithContent($"Want me to add this to the quote DB?\n>>> {targetMsg.Content}")
            .AddComponents((IEnumerable<DiscordActionRowComponent>)new[]
            {
                bleh
            })
            .AsEphemeral();

        await ctx.CreateResponseAsync(interactionResponse);
        var msg = await ctx.GetOriginalResponseAsync();

        var choice = await inter.WaitForButtonAsync(msg);

        if (choice.Result.Interaction.Data.CustomId == "yes")
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Working."));

            var q = new Quote
            {
                DiscordSnowFlake = targetMsg.Id,
                QuoteText = targetMsg.Content,
                QuoteeName = targetMsg.Author.Username,
                QouteeDiscordSnowflake = targetMsg.Author.Id,

            };
            _db.Add(q);
            await _db.SaveChangesAsync();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Done!"));
        }
        else
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No good after all?"));
        }
    }

    private string MentionTron(Match x)
    {
        try
        {
            //ugly asyc boundary because System.Regex dont do async yet.
            return "@" + _client.GetUserAsync(ulong.Parse(x.Groups["id"].ToString())).Result.Username;
        }
        catch
        {
            return "@" + "███████████";
        }
    }
}