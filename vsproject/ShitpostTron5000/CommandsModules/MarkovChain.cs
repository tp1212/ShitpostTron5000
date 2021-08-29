using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShitpostTron5000.CommandsModules
{
    [ModuleLifespan(ModuleLifespan.Singleton)]
    class MarkovChain : BaseCommandModule
    {
        private MarkovChain<string> _markov;
        private Random _random;

        private List<Bookmark> _bookMarks = new List<Bookmark>();

        public MarkovChain(MarkovChain<string> markov, Random random)
        {
            _markov = markov;
            _random = random;
        }


        [Command("markov")]
        [Aliases("marcov")]
        [Description("Reads the last 100 messages into the current markov chain and spits something out.")]
        public async Task Markov(CommandContext ctx, [RemainingText] string prompt = null)
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.RespondAsync("I'm not reading any of that, try again in a sfw channel.");
                return;
            }
            if (ctx.Channel.Id == 425319177738518531)
            {
                await ctx.RespondAsync("Marcov 'chain', not marcov web. (no spiders!)");
                return;
            }

            var bookmark = _bookMarks.FirstOrDefault(x => x.Channel == ctx.Channel.Id)
                           ?? new Bookmark()
                           {
                               Channel = ctx.Channel.Id,
                               Newest = ctx.Message.Id,
                               Oldest = ctx.Message.Id
                           };
            

            var messages = (await ctx.Channel.GetMessagesBeforeAsync(bookmark.Oldest))
                    .Concat(await ctx.Channel.GetMessagesAfterAsync(bookmark.Newest))
                .OrderBy(x => x.Timestamp)
                .ToList();
            bookmark.Newest = messages.First().Id;
            bookmark.Oldest = messages.Last().Id;


            var filteredMessages = messages
                .Where(x => !x.Author.IsBot)
                .Where(x => !x.Content.StartsWith("!"));

            var sentences = filteredMessages
                .Select(x => x.Content.Split(' '));

            foreach (var sentence in sentences)
            {
                _markov.Add(sentence);
            }

            for (var i = 0; i < 10; i++)
            {
                string shitpost;
                if (prompt != null)
                {
                    shitpost = $"{prompt} {string.Join(' ', _markov.Chain(prompt.Split(' '), _random))}";
                }
                else
                {
                    shitpost = string.Join(' ', _markov.Chain(_random));
                }

                if (shitpost.Length == 0)
                {
                    continue;
                }
                await ctx.RespondAsync(shitpost);
                return;
            }
            await ctx.RespondAsync("Not feeling it I guess? (marcov chain resulted in empty string)");
        }

        private class Bookmark
        {
            public ulong Newest { get; set; }
            public ulong Oldest { get; set; }
            public ulong Channel { get; set; }
        }
    }
}
