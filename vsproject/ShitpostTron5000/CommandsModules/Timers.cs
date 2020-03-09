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
            x=>$"{x.Member.Mention}, your timer has expired.",
            x=>$"RING RING MOTHERFUCKER {x.Member.Mention}!",
            x=>$"Still Somehow less finicky than the google one eh? {x.Member.Mention}!",
            x=>$"Discord API latency not included {x.Member.Mention}!",
            x=>$"By using this timer service you rescind your rights to complain about time {x.Member.Mention}!",
            x=>$"Its {DateTime.Now:HH:MM:SS} time to go bother {x.Member.Mention}!",
            x=>$"Whatever you needed to get doing {x.Member.Mention} its time to go do it.",
        };

        static readonly List<Func<CommandContext, string, string>> NamedTimerResponses = new List<Func<CommandContext, string, string>>
        {
            (x,y)=>$"{x.Member.Mention}, your timer {y} has expired.",
            (x,y)=>$"My liege {x.Member.Mention}, {y} has happened to us!",
            (x,y)=>$"{x.Member.Mention}, {y}!",
            (x,y)=>$"{x.Member.Mention}, whatever {y} is. its something you should respond to.",
            (x,y)=>$"Ah {x.Member.Mention}, and their petty worries about {y}.",
            (x,y)=>$"{y} is a thing I associate with {x.Member.Mention} all of a sudden right now.",
        };



        void Startup()
        {

        }



        [Command("timer")]
        [Description("sets a timer. format is 00000d00h00m00s")]
        public async Task TimerCmd(CommandContext ctx, TimeSpan span, string name = null)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));

            if (span >= TimeSpan.FromHours(2))
                await ctx.RespondAsync($"You know {ctx.Member.Nickname}, if I get rebooted I'm not gonna remember that shit, but I'll try.");


            await Task.Delay(span);

            await ctx.RespondAsync(name?.ToLower() switch
            {
                "fart" => $"poot. {ctx.Member.Mention}",
                "bomb" => $"{ctx.Member.Mention}, I really hated those google search suggestions on the word timer.",
                "timer" => $"{ctx.Member.Mention}, your timer timer... why did you name your timer timer?",
                "ping" => $"{ctx.Member.Mention}, your ping is {span.TotalMilliseconds} ms.",
                "coronavirus" => $"{ctx.Member.Mention}, *cough*",
                "corona" => $"{ctx.Member.Mention}, *cough*",
                "death" => $"We are gathered here today to remember the passing of {ctx.Member.Mention}.",
                "die" => $"{ctx.Member.Mention}, this is your {DateTime.Now:HH} o'clock reminder to die.",
                "stop" => $"{ctx.Member.Mention}, not stoppin.",
                "win" => $"{ctx.Member.Mention} gets no prizes.",
                null => TimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx), //lazy way to pick one at random.
                string y => NamedTimerResponses.OrderBy(x => Guid.NewGuid()).First()(ctx, name)
            }
            );
        }
    }
}
