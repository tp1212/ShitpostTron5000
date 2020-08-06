using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitpostTron5000.CommandsModules
{
    class Timers
    {

        static readonly List<Func<CommandContext, string>> TimerResponses = new List<Func<CommandContext, string>>
        {
            x=>$"{MemberMentionOrName(x)}, your timer has expired.",
            x=>$"RING RING MOTHERFUCKER {MemberMentionOrName(x)}!",
            x=>$"Still Somehow less finicky than the google one eh? {MemberMentionOrName(x)}!",
            x=>$"Discord API latency not included {MemberMentionOrName(x)}!",
            x=>$"By using this timer service you rescind your rights to complain about time {MemberMentionOrName(x)}!",
            x=>$"Its {DateTime.Now:HH:MM:SS} time to go bother {MemberMentionOrName(x)}!",
            x=>$"Whatever you needed to get doing {MemberMentionOrName(x)} its time to go do it.",
        };

        private static string MemberMentionOrName(CommandContext ctx)
        {
            return ctx.Member?.Mention ?? ctx.User.Username;
        }

        static readonly List<Func<CommandContext, string, string>> NamedTimerResponses = new List<Func<CommandContext, string, string>>
        {
            (x,y)=>$"{MemberMentionOrName(x)}, your timer {y} has expired.",
            (x,y)=>$"My liege {MemberMentionOrName(x)}, {y} has happened to us!",
            (x,y)=>$"{MemberMentionOrName(x)}, {y}!",
            (x,y)=>$"{MemberMentionOrName(x)}, whatever {y} is. its something you should respond to.",
            (x,y)=>$"Ah {MemberMentionOrName(x)}, and their petty worries about {y}.",
            (x,y)=>$"{y} is a thing I associate with {MemberMentionOrName(x)} all of a sudden right now.",
        };



        void Startup()
        {

        }



        [Command("timer")]
        [Description("sets a timer. format is 00000d00h00m00s")]
        public async Task TimerCmd(CommandContext ctx, TimeSpan span, [RemainingText]string name = null)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));

            if (span >= TimeSpan.FromHours(2))
                await ctx.RespondAsync($"You know {ctx.Member.Nickname}, if I get rebooted I'm not gonna remember that shit, but I'll try.");


            await Task.Delay(span);

            await ctx.RespondAsync(name?.ToLower() switch
            {
                "fart" => $"poot. {MemberMentionOrName(ctx)}",
                "bomb" => $"{MemberMentionOrName(ctx)}, I really hated those google search suggestions on the word timer.",
                "timer" => $"{MemberMentionOrName(ctx)}, your timer timer... why did you name your timer timer?",
                "ping" => $"{MemberMentionOrName(ctx)}, your ping is {span.TotalMilliseconds} ms.",
                "coronavirus" => $"{MemberMentionOrName(ctx)}, *cough*",
                "corona" => $"{MemberMentionOrName(ctx)}, *cough*",
                "death" => $"We are gathered here today to remember the passing of {MemberMentionOrName(ctx)}.",
                "die" => $"{MemberMentionOrName(ctx)}, this is your {DateTime.Now:HH} o'clock reminder to die.",
                "stop" => $"{MemberMentionOrName(ctx)}, not stoppin.",
                "win" => $"{MemberMentionOrName(ctx)} gets no prizes.",
                null => TimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx), //lazy way to pick one at random.
                { } y => NamedTimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx, name)
            }
            );
        }
    }
}
