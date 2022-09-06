
using Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;


namespace ShitpostTron5000.CommandsModules
{
    [SlashModuleLifespan(SlashModuleLifespan.Singleton)]
    class MarkovChain : ApplicationCommandModule
    {
        private MarkovChain<string> _markov;
        private Random _random;

        private List<Bookmark> _bookMarks = new List<Bookmark>();

        public MarkovChain(MarkovChain<string> markov, Random random)
        {
            _markov = markov;
            _random = random;
        }


        [SlashCommand("markov", "dumps 100 messages into the current markov chain and spits something out.")]
        public async Task Markov(InteractionContext ctx, [Option("prompt", "a starting point for the chain, you can use more than one word but it only uses the last")] string prompt = "")
        {
            if (ctx.Channel.IsNSFW)
            {
                await ctx.CreateResponseAsync("I'm not reading any of that, try again in a sfw channel.");
                return;
            }
            if (ctx.Channel.Id == 425319177738518531)
            {
                await ctx.CreateResponseAsync("Marcov 'chain', not marcov web. (no spiders!)");
                return;
            }

            if (ctx.Channel.LastMessageId is null)
            {
                await ctx.CreateResponseAsync("this channel is fucking empty?");
                return;
            }

            var bookmark = _bookMarks.FirstOrDefault(x => x.Channel == ctx.Channel.Id)
                           ?? new Bookmark()
                           {
                               Channel = ctx.Channel.Id,
                               Newest = ctx.Channel.LastMessageId.Value,
                               Oldest = ctx.Channel.LastMessageId.Value
                           };

            var messages = (await ctx.Channel.GetMessagesBeforeAsync(bookmark.Oldest))
                    .Concat(await ctx.Channel.GetMessagesAfterAsync(bookmark.Newest))
                .OrderBy(x => x.Timestamp)
                .ToList();
            bookmark.Newest = messages.First().Id;
            bookmark.Oldest = messages.Last().Id;

            var filteredMessages = messages
                .Where(x => !x.Author.IsBot)
                .Where(x => !x.Content.StartsWith("!")); //old bot commands, probably.

            var sentences = filteredMessages
                .Select(x => x.Content.Split(' '));

            foreach (var sentence in sentences)
            {
                _markov.Add(sentence);
            }

            for (var i = 0; i < 40; i++)
            {
                var words = !string.IsNullOrEmpty(prompt)
                    ? _markov.Chain(prompt.Split(' ',StringSplitOptions.RemoveEmptyEntries), _random)
                    : _markov.Chain(_random);

                var shitpost = string.Join(' ', words);

                if (string.IsNullOrWhiteSpace(shitpost))
                {
                    continue;
                }

                await ctx.CreateResponseAsync(string.Join(' ', prompt, shitpost));

                return;
            }

            await ctx.CreateResponseAsync("Not feeling it I guess? (marcov chain resulted in empty string)", true);
        }

        private class Bookmark
        {
            public ulong Newest { get; set; }
            public ulong Oldest { get; set; }
            public ulong Channel { get; set; }
        }
    }
}
