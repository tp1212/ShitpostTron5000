using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Serilog;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{
    class QuoteDB
    {

        [Command("randomQuote")]
        public async Task GetRandomQuote(CommandContext ctx)
        {
            ShitpostTronContext db = Program.GetDbContext();
            if(!db.Quotes.Any())
                return;
            

            var qoteNumber = new Random().Next(db.Quotes.Count());
            var result = db.Quotes.Skip(qoteNumber).First();

            await ctx.RespondAsync($">>> {result.QuoteText}");
        }
        


    [Command("quote")]
        public async Task QuoteFromUser(CommandContext ctx, DiscordMember qotee, int skip =0)
        {
            var candidates = await ctx.Channel.GetMessagesAsync(50);
            var targetMsg = candidates.Where(x => x.Author == qotee)
                .Skip(skip)
                .FirstOrDefault();
            if (targetMsg == null)
            {
                await ctx.RespondAsync($"I cant find a recent message by that user{(skip!=0?$" or I skipped it.":"")}.");
                return;
            }

            var inter = Program.Client.GetInteractivityModule();

            var message =  await ctx.RespondAsync($"Want me to add this to the quote DB?\n>>> {targetMsg.Content}");

            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("❎"));

           


            var choice = await inter.WaitForReactionAsync(x => x.Name.Contains("✅",
                                                                   StringComparison.OrdinalIgnoreCase) ||
                                                               x.Name.Contains("❎",
                                                                   StringComparison.OrdinalIgnoreCase),
                ctx.User);
            try
            {
                await message.DeleteAllReactionsAsync();
            }
            catch (Exception ex)
            {
                Log.Warning("did not clean reactons",ex);
            }
            

            if (choice.Emoji.Name.Contains("✅", StringComparison.OrdinalIgnoreCase))
            {
                await message.ModifyAsync("Working.");
                ShitpostTronContext db = Program.GetDbContext();

                Quote q = new Quote
                {
                    DiscordSnowFlake = targetMsg.Id,
                    QuoteText = targetMsg.Content,
                    QuoteeName = targetMsg.Author.Username,
                    QouteeDiscordSnoflake = targetMsg.Author.Id,

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
            }
        }
    }
}