using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Blehgh
{
    class BasicCommands
    {
        private static Random _rand = new Random();

        [Command("say")]
        public async Task Say(CommandContext ctx)
        {
            await ctx.RespondAsync(ctx.RawArgumentString);
            Console.WriteLine($"I had to say {ctx.RawArgumentString}");
        }

        [Command("roll")]
        public async Task Roll(CommandContext ctx, int dice, int dicen)
        {
            StringBuilder response = new StringBuilder();
            response.Append($"Rolling {dice} d{dicen}ns\n");
            response.Append("Your rolls are:```\n");
            for (int i = 1; i < dice + 1; i++)
            {
                response.Append($"{i}:{_rand.Next(0, dicen)}\t");
                if (i % 10 == 0) response.Append($"\n");
            }
            response.Append("```");
            await ctx.RespondAsync(response.ToString());
            Console.WriteLine($"I had to roll dice.");
        }


        [Command("status")]
        public async Task Status(CommandContext ctx)
        {

            StringBuilder response = new StringBuilder();
            response.Append($"I have been online since {Program.Start}  (uptime:{DateTime.Now - Program.Start})\n");
            response.Append($"My ping is {Program.Client.Ping}\n");
            response.Append($"I am connected to {Program.Client.Guilds.Count} Servers\n");

            response.Append($"I am aware of the following channels on this server: ```\n");
            foreach (DiscordChannel chanl in ctx.Guild.Channels)
            {
                response.Append($"{chanl.Name}:{chanl.Id} NSFW:{(chanl.IsNSFW ? "YES" : "NO")}\n");
            }
            response.Append("```");




              await ctx.RespondAsync(response.ToString());
            Console.WriteLine($"I gave a status update.");
        }
    }
}
