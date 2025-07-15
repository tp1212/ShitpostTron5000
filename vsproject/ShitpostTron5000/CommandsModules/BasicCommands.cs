using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace ShitpostTron5000.CommandsModules
{
    [SlashModuleLifespan(SlashModuleLifespan.Transient)]
    class BasicCommands : ApplicationCommandModule
    {
        private Random _random;
        private readonly DiscordClient _client;

        public BasicCommands(Random random, DiscordClient client)
        {
            _random = random;
            _client = client;
        }

        [SlashCommand("stop", "STOP IT.")]
        [Description()]
        public async Task Stop(InteractionContext ctx)
        {
            var content = string.Concat(Enumerable.Repeat("STOP IT, ", 100));
            await ctx.CreateResponseAsync(content);
            await ctx.Member.SendMessageAsync(content);
        }


        [SlashCommand("say", "Make me bot say a thing.")]
        public async Task Say(InteractionContext ctx, [Option("text", "the text of the say command")] string text)
        {

            await ctx.CreateResponseAsync(text);
            Console.WriteLine($"I had to say {text}");
        }

        [SlashCommand("flip", "Flip the bot's avatar. (please dont spam this one)")]
        public async Task Flip(InteractionContext ctx)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"ShitpostTron5000.Assets.ratava.png"))
            {
                var user = await _client.UpdateCurrentUserAsync(avatar: stream);
                await ctx.CreateResponseAsync("flip!");
            }
        }


        [SlashCommand("unflip", "Unflip the bot's avatar. (please dont spam this one either)")]
        public async Task Unflip(InteractionContext ctx)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"ShitpostTron5000.Assets.avatar.png"))
            {
                await _client.UpdateCurrentUserAsync(avatar: stream);
                await ctx.CreateResponseAsync(new string("flip!".Reverse().ToArray()));
            }
        }


        [SlashCommand("freud", "Ring ring")]
        public async Task Freuddian(InteractionContext ctx)
        {

            await ctx.CreateResponseAsync("Ring... Ring...");

            const string emj = "☎";
            var discordClient = _client;
            var message = await ctx.GetOriginalResponseAsync();
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode(discordClient, emj));


            while (true)
            {
                var reacts = await discordClient.GetInteractivity()
                    .WaitForReactionAsync(x =>
                            x.Emoji.Name == emj,
                        TimeSpan.FromMinutes(5));
                
                if (reacts.TimedOut)
                    return;
                if (reacts.Result.User == discordClient.CurrentUser)
                    continue;
                if (reacts.Result.Message != message)
                    continue;
                var reactsMember = await ctx.Guild.GetMemberAsync(reacts.Result.User.Id);              
                await reactsMember.SendMessageAsync("Kill zem, Kill zem all...");
            }
        }


        [SlashCommand("lottery", "Draw some lots")]
        public async Task Lots(InteractionContext ctx, [Option("winners", "Amount of 'winners'")] long drawCount = 1, [Option("Wait", "Time to wait, in seconds")] long timeOutInSeconds = 60)
        {
            var sassBuilder = new StringBuilder();

            if (drawCount < 0)
            {
                await ctx.CreateResponseAsync("Is this some attempt to undo a previous draw or something? Sorry but you're stuck with it.");
                return;
            }
            if (timeOutInSeconds < 0)
            {
                await ctx.CreateResponseAsync("I have announced a winner for that draw in an alternate timeline for you.");
                return;
            }

            var emoji = ctx.Guild.Emojis.Values.OrderBy(x => Guid.NewGuid()).FirstOrDefault() ?? DiscordEmoji.FromUnicode("✅");

            await ctx.CreateResponseAsync($"Drawing lots, use the {emoji} reaction to join.");
            var msg = await ctx.GetOriginalResponseAsync();
            await msg.CreateReactionAsync(emoji);

            List<DiscordUser> participants = new List<DiscordUser>();
            await Task.Delay(TimeSpan.FromSeconds(timeOutInSeconds));
            var users = (await msg.GetReactionsAsync(emoji)).Where(x => !x.IsBot);

            var winnars = users.OrderBy(x => Guid.NewGuid()).Take((int)drawCount).ToList();
            var losars = users.Where(x => !x.IsBot).Except(winnars).ToList();


            await msg.ModifyAsync(
                (winnars.Any() ? $"And we have some \"lucky\" winners:{string.Join(' ', winnars.Select(x => x.Mention))}\n" : "No winners on this draw.\n") +
                (losars.Any() ? $"We also have some probably even more lucky losers: {string.Join(' ', losars.Select(x => x.Mention))}" : "No losers on this draw?")
                );

        }


        [SlashCommand("status", "Status report.")]
        public async Task Status(InteractionContext ctx)
        {

            StringBuilder response = new StringBuilder();
            response.Append($"I have been online since {Program.Start}  (uptime:{DateTime.Now - Program.Start})\n");
            response.Append($"My ping is {_client.Ping}\n");
            response.Append($"I am connected to {_client.Guilds.Count} Servers\n");

            response.Append($"I am aware of the following channels on this server: ```\n");
            foreach (DiscordChannel chanl in ctx.Guild.Channels.Values)
            {
                response.Append($"{chanl.Name}:{chanl.Id} NSFW:{(chanl.IsNSFW ? "YES" : "NO")}\n");
            }
            response.Append("```");

            await ctx.CreateResponseAsync(response.ToString());
            Console.WriteLine($"I gave a status update.");
        }

        [SlashCommand("move", "This moves someone to any channel. I should not be giving you this power.")]
        public async Task Move(InteractionContext ctx, [Option("who", "Who are you bothering")] DiscordUser user, [Option("where", "where are you dumping this poor soul?")][ChannelTypes(ChannelType.Voice)]DiscordChannel target)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (member.VoiceState.Channel.Guild == ctx.Guild)
            {
                await member.ModifyAsync((member) => member.VoiceChannel = target);
            }
            else
            {
                await ctx.CreateResponseAsync("no.");
                return;
            }
            await ctx.CreateResponseAsync("done!");

        }


    }
}
