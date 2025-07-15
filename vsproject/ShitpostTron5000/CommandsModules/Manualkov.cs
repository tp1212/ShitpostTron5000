
using Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;


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
        public async Task Markov(InteractionContext ctx, [Option("prompt", "a starting point for the chain.")] string prompt = "")
        {
            if(ctx.Guild == null)
            {
                await ctx.CreateResponseAsync("Well, this isnt a server, so this wont work. sorry.");
                return;
            }

            await ctx.CreateResponseAsync("Okay, starting it now!",true);
 
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
                .OrderBy(x=>Guid.NewGuid())
                .ToList();

            var length = participants.Count() + _random.Next(5);
            var participantIndex = 0;

            while (words.Count() < length)
            {
                var nextParticipant = participants[participantIndex++%participants.Count()];
                var member = await ctx.Guild.GetMemberAsync(nextParticipant.Id);
                var talky = await member.CreateDmChannelAsync();

                var currentWordDetails = $"The current word is '{words.LastOrDefault()}', continue on from here.";

                var details = (words.Count == 0, words.Count == length) switch 
                {
                    (true,false) => "You are picking the first word on the chain, so capitalize it and stuff",
                    (false,true) => $"You are picking the last word on the chain, so feel free to punctuate!\n{currentWordDetails}",
                    _ => $"You are picking a middle word on the chain, so it should not have capitalisation and punctuation (unless you want it to)\n{currentWordDetails}."
                };

                string memberMessage = $"You are the next in the markov chain.\n" +
                    $"{details}\n" +
                    $"I will read the next DM you send to me and I will use the contents of that message to add to the chain. (Try to keep it one word/phrase I guess?)";

                await talky.SendMessageAsync(memberMessage);

                var response = await talky.GetNextMessageAsync();
                if(response.TimedOut)
                {
                    await talky.SendMessageAsync("You took too long, skipped your turn!");
                    continue;
                }
                words.Add(response.Result.Content);
                await talky.SendMessageAsync("Great contribution!");
            }
            var result = string.Join(" ", words);
            await ctx.Channel.SendMessageAsync(result);            
        }
    }
}
