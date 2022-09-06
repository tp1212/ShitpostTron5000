using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace ShitpostTron5000.CommandsModules
{
    [SlashModuleLifespan(SlashModuleLifespan.Transient)]
    class Timers : ApplicationCommandModule
    {

        static readonly List<Func<InteractionContext, string>> TimerResponses = new()
        {
            x => $"{MemberMentionOrName(x)}, your timer has expired.",
            x => $"RING RING MOTHERFUCKER {MemberMentionOrName(x)}!",
            x => $"Still Somehow less finicky than the google one eh? {MemberMentionOrName(x)}!",
            x => $"Discord API latency not included {MemberMentionOrName(x)}!",
            x => $"By using this timer service you rescind your rights to complain about time {MemberMentionOrName(x)}!",
            x => $"Its {DateTime.Now:HH:MM:SS} time to go bother {MemberMentionOrName(x)}!",
            x => $"Whatever you needed to get doing {MemberMentionOrName(x)} its time to go do it.",
        };

        private static string MemberMentionOrName(InteractionContext ctx)
        {
            return ctx.Member?.Mention ?? ctx.User.Username;
        }

        static readonly List<Func<InteractionContext, string, string>> NamedTimerResponses = new()
        {
            (x, y) => $"{MemberMentionOrName(x)}, your timer {y} has expired.",
            (x, y) => $"My liege {MemberMentionOrName(x)}, {y} has happened to us!",
            (x, y) => $"{MemberMentionOrName(x)}, {y}!",
            (x, y) => $"{MemberMentionOrName(x)}, whatever {y} is. its something you should respond to.",
            (x, y) => $"Ah {MemberMentionOrName(x)}, and their petty worries about {y}.",
            (x, y) => $"{y} is a thing I associate with {MemberMentionOrName(x)} all of a sudden right now.",
        };



        void Startup()
        {

        }



        [SlashCommand("timer",
            "sets a timer.")]
        public async Task TimerCmd(InteractionContext ctx,
            [Option("Minutes", "How many minutes to wait.")]
            long minutes = 0,
            [Option("Hours", "How many hours to wait.")]
            long hours = 0,
            [Option("Days",
                "How many hours to wait.")]
            long days = 0,
            [Option("name", "The thing you want to be reminded of")]
            string name = null)
        {
            var target = DateTimeOffset.Now.AddMinutes(minutes).AddHours(hours).AddDays(days);
            var targetSpan = target - DateTime.Now;

            await ctx.CreateResponseAsync($"I set a timer for <t:{target.ToUnixTimeSeconds()}>, keep in mind I don't save these on disk, so I might forget",true);
            
            await Task.Delay(targetSpan);

            await ctx.Channel.SendMessageAsync(name?.ToLower() switch
            {
                "fart" => $"poot. {MemberMentionOrName(ctx)}",
                "bomb" => $"{MemberMentionOrName(ctx)}, I really hated those google search suggestions on the word timer.",
                "timer" => $"{MemberMentionOrName(ctx)}, your timer timer... why did you name your timer timer?",
                "ping" => $"{MemberMentionOrName(ctx)}, your ping is {targetSpan.TotalMilliseconds} ms.",
                "coronavirus" => $"{MemberMentionOrName(ctx)}, *cough*",
                "corona" => $"{MemberMentionOrName(ctx)}, *cough*",
                "death" => $"We are gathered here today to remember the passing of {MemberMentionOrName(ctx)}.",
                "die" => $"{MemberMentionOrName(ctx)}, this is your {DateTime.Now:HH} o'clock reminder to die.",
                "stop" => $"{MemberMentionOrName(ctx)}, i am not stoppin.",
                "win" => $"{MemberMentionOrName(ctx)} gets no prizes.",
                null => TimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx), //lazy way to pick one at random.
                { } y => NamedTimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx, name)
            }
            );
        }
    }
}
