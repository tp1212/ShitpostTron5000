
using Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using System.Threading;


namespace ShitpostTron5000.CommandsModules
{
    [SlashModuleLifespan(SlashModuleLifespan.Transient)]
    class Manualkov : ApplicationCommandModule
    {
        private Random _random;

        public Manualkov(Random random)
        {
            _random = random;
        }

        [SlashCommand("manualkov", "Recruit your fellow server members to generate a string.")]
        public async Task Markov(InteractionContext ctx, [Option("prompt", "a starting point for the chain.")] string prompt = "", [Option("Lookback", "The amount of words you can see at the end of the chain.")] long longlookback = 1)
        {
            int lookback = (int)longlookback;

            if (ctx.Guild == null)
            {
                await ctx.CreateResponseAsync("Well, this isnt a server, so this wont work. sorry.");
                return;
            }

            await ctx.CreateResponseAsync("Okay, starting it now!", true);

            var message = await ctx.Channel.SendMessageAsync($"We are doing a markov chain, *together*!\n" +
                 $"Please react to this message to join!\n" +
                 $"Starting in 2 minutes!");

            List<string> words = [];

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                words.Add(prompt);
            }

            var reactions = await message.CollectReactionsAsync(TimeSpan.FromMinutes(2));

            var participants = reactions.SelectMany(x => x.Users)
                .Distinct()
                .OrderBy(x => Guid.NewGuid())
                .ToList();

            try
            {
                var participantIndex = 0;

                while (true)
                {
                    await message.ModifyAsync("Chain in progress." + new string('.', words.Count));

                    var nextParticipant = participants[participantIndex++ % participants.Count()];
                    var member = await ctx.Guild.GetMemberAsync(nextParticipant.Id);
                    var talky = await member.CreateDmChannelAsync();

                    var currentWordDetails = $"You should continue on from '{string.Join(" ",words.TakeLast(2))}', there is {words.Count} words in the current result and there's {participants.Count} players.";

                    var details = (words.Count == 0) switch
                    {
                        true => "You are picking the first word on the chain.",
                        _ => $"{currentWordDetails}.\n" +
                        $"if you want to end the chain, add END to your message. (Yes, all caps)"
                    };

                    string memberMessage = $"{details}\n" +
                        $"I will read the next DM you send to me and add it to the chain. (Try to keep it one word I guess?)\n";

                    await talky.SendMessageAsync(memberMessage);

                    var response = await talky.GetNextMessageAsync(TimeSpan.FromSeconds(70));
                    if (response.TimedOut)
                    {
                        await talky.SendMessageAsync("You took too long, skipped your turn!");
                        continue;
                    }
                    var responseText = response.Result.Content;

                    if (responseText.Contains("END"))
                    {
                        words.Add(response.Result.Content.Replace("END", "").Trim());
                        break;
                    }

                    words.Add(response.Result.Content.Trim());
                    await talky.SendMessageAsync("Great contribution!");
                }
            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Something went wrong, so the chain got ended early. Heres what I got though" + string.Join(" ", words));
                throw;
            }
            var result = string.Join(" ", words);
            await ctx.Channel.SendMessageAsync(result);
        }
    }
}
