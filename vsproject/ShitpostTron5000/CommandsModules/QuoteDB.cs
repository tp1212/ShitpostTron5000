using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Serilog;
using ShitpostTron5000.Data;

namespace ShitpostTron5000.CommandsModules
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class QuoteDB : BaseCommandModule
    {
        private readonly ShitpostTronContext _db;
        private readonly InteractivityExtension _interactivity;
        private readonly DiscordClient _client;

        public QuoteDB(ShitpostTronContext db, InteractivityExtension interactivity,DiscordClient client)
        {
            _db = db;
            _interactivity = interactivity;
            _client = client;
        }


        [Command("QuoteRandom")]
        [Description("Get a quote at random, yay.")]
        public async Task GetRandomQuote(CommandContext ctx)
        {
            if (!_db.Quotes.Any())
                return;


            var quoteNumber = new Random().Next(_db.Quotes.Count());
            await SayQuote(ctx,  quoteNumber);
        }

        [Command("Quote")]
        [Description("Get a quote by number, yay.")]
        public async Task GetQuote(CommandContext ctx, int number)
        {
            if (!_db.Quotes.Any())
                return;

            await SayQuote(ctx, number);
        }

        [Command("QuoteBrowser")]
        [Description("Get a quote by number, yay.")]
        public async Task GetPaginatedQuote(CommandContext ctx, int number = 1)
        {
            
            var inter = _client.GetInteractivity();

            await inter.SendPaginatedMessageAsync(ctx.Channel,
                ctx.User,
                _db.Quotes.Select(x => new Page(QuoteToString(x), null)));

        }

        private async Task SayQuote(CommandContext ctx, int quoteNumber)
        {
            var result = _db.Quotes.FirstOrDefault(x => x.Id == quoteNumber);
            if (result == null)
            {
                ctx.RespondAsync($"Could not find quote #{quoteNumber}");
            }

            await ctx.RespondAsync(QuoteToString(result));
        }

        private static string QuoteToString(Quote result)
        {
            return $"{result.QuoteText.Replace("\n", "\n> ")} \n―{result.QuoteeName} \t#{result.Id}";
        }


        [Command("QuoteManual")]
        [Description("Manually add a quote, like for when you hear something\n eg !quoteManual @Steve Funny words go here.")]
        public async Task QuoteFromUser(CommandContext ctx, DiscordMember attributeToMember, [RemainingText] string Content)
        {
            var attributeToName = attributeToMember.DisplayName;

            var mention = new Regex("<@!(?<id>\\d+)>");

            Content = mention.Replace(Content, MentionTron);

            var message = await ctx.RespondAsync($"Want me to add this to the quote DB?\n> {Content.Replace("\n", "\n> ")} \n―{attributeToName}");

            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("❎"));

            var choice = await _interactivity.WaitForReactionAsync(x => x.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase)
                                                               || x.Emoji.Name.Contains("❎", StringComparison.OrdinalIgnoreCase),
                ctx.User);

            try
            {
                await message.DeleteAllReactionsAsync();
            }
            catch (Exception ex)
            {
                Log.Warning("did not clean reactons", ex);
            }

            if (choice.Result.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase))
            {
                await message.ModifyAsync("Working.");
                

                Quote q = new Quote
                {
                    DiscordSnowFlake = 0,
                    QuoteText = Content,
                    QuoteeName = attributeToName,
                    QouteeDiscordSnowflake = attributeToMember.Id,

                };
                _db.Add(q);
                await _db.SaveChangesAsync();
                await message.ModifyAsync("Done!");

            }
            else
            {
                await message.ModifyAsync("No good after all?");
            }
            await Task.Delay(TimeSpan.FromSeconds(6));
            await message.DeleteAsync();
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

        [Command("QuoteChat")]
        public async Task QuoteChat(CommandContext ctx, DiscordMember qotee, int skip = 0)
        {
            var candidates = await ctx.Channel.GetMessagesAsync(50);
            var targetMsg = candidates.Where(x => x.Author == qotee)
                .Skip(skip)
                .FirstOrDefault();
            if (targetMsg == null)
            {
                await ctx.RespondAsync($"I cant find a recent message by that user{(skip != 0 ? $" or I skipped it." : "")}.");
                return;
            }

            var inter = _client.GetInteractivity();

            var message = await ctx.RespondAsync($"Want me to add this to the quote DB?\n>>> {targetMsg.Content}");

            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("❎"));

            var choice = await inter.WaitForReactionAsync(x => x.Emoji.Name.Contains("✅",
                                                                   StringComparison.OrdinalIgnoreCase) ||
                                                               x.Emoji.Name.Contains("❎",
                                                                   StringComparison.OrdinalIgnoreCase),
                ctx.User);
            try
            {
                await message.DeleteAllReactionsAsync();
            }
            catch (Exception ex)
            {
                Log.Warning("did not clean reactons", ex);
            }


            if (choice.Result.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase))
            {
                await message.ModifyAsync("Working.");
                ShitpostTronContext db = Program.GetDbContext();

                Quote q = new Quote
                {
                    DiscordSnowFlake = targetMsg.Id,
                    QuoteText = targetMsg.Content,
                    QuoteeName = targetMsg.Author.Username,
                    QouteeDiscordSnowflake = targetMsg.Author.Id,

                };
                db.Add(q);
                await db.SaveChangesAsync();
                await message.ModifyAsync("Done!");
                await Task.Delay(TimeSpan.FromSeconds(6));
                await message.DeleteAsync();
            }
            else
            {
                await message.ModifyAsync("No good after all?");
                await Task.Delay(TimeSpan.FromSeconds(6));
                await message.DeleteAsync();
            }
        }
    }
}