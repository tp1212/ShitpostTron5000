using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace ShitpostTron5000
{
    class BasicCommands
    {
        private static Random _rand = new Random();
        private readonly QuoteDB QuoteDB = new QuoteDB();

        [Command("stop")]
        [Hidden]
        [Description("STOP IT.")]
        public async Task Stop(CommandContext ctx)
        {
            string content = string.Concat(Enumerable.Repeat("STOP IT, ", 100));
            await ctx.RespondAsync(content);
            DiscordChannel pm = await Program.Client.CreateDmAsync(ctx.User);
            await pm.SendMessageAsync(content);
            Console.WriteLine($"I had to stop it, I did not.");
        }


        [Command("say")]
        [Description("Make me bot say a thing.")]
        public async Task Say(CommandContext ctx, [RemainingText][Description("The message you want to say")]string text)
        {
            try
            {
                await ctx.Message.DeleteAsync();
            }
            catch 
            { }
            await ctx.RespondAsync(text);
            Console.WriteLine($"I had to say {text}");
        }


        [Command("ьщму")]
        [Description("Мувес Неткэв то юур воис чаннэл.")]
        [Hidden]
        public async Task SummonKevinSpecifically(CommandContext ctx)
        {

            if (ctx.Member.VoiceState.Channel == null)
                return;

            var kevin = await Program.Client.GetUserAsync(91586237478998016);


            var memberkevin = ctx.Guild.Members.FirstOrDefault(x => x.Username == kevin.Username);
            if (memberkevin?.VoiceState?.Channel == null)
                return;

            await memberkevin.PlaceInAsync(ctx.Member.VoiceState.Channel);

            await ctx.RespondAsync($"ьщму {kevin.Mention}! ьщму!");
        }


        [Command("flip")]
        [Hidden]
        public async Task flip(CommandContext ctx)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"ShitpostTron5000.Assets.ratava.png"))
            {
                await Program.Client.EditCurrentUserAsync(avatar: stream);
            }
        }



        [Command("unflip")]
        [Hidden]
        public async Task unflip(CommandContext ctx)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"ShitpostTron5000.Assets.avatar.png"))
            {
                await Program.Client.EditCurrentUserAsync(avatar: stream);
            }
        }


        [Command("freud")]
        [Hidden]
        public async Task Freuddian(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("Ring... Ring...");
            const string emj = "☎";
            var discordClient = Program.Client;
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode(discordClient, emj));


            while (true)
            {
                var reacts = await discordClient.GetInteractivityModule()
                    .WaitForReactionAsync(x =>
                            x.Name == emj, 
                        TimeSpan.FromMinutes(5));
                if (reacts == null)
                    return;
                if (reacts.User == discordClient.CurrentUser)
                    continue;

                if (reacts.Message != message)
                    continue;
                var dm = await discordClient.CreateDmAsync(reacts.User);

                await dm.SendMessageAsync("Kill zem, Kill zem all...");
            }
        }



        [Command("roll")]
        [Description("roll some dice, example: !roll 10d5 3d4-2")]
        public async Task Roll(CommandContext ctx, [Description("#d#[(+|-)#]")] params string[] dicestrings)
        {
            Console.WriteLine($"I had to roll dice.");
            StringBuilder response = new StringBuilder();

            if (ctx.Message.Content.Contains("#d#[(+|-)#]"))
            {
                await ctx.RespondAsync("Smartass.");
                return;
            }


            List<(int dc, int dn, int dm)> dicelist = new List<(int dc, int dn, int dm)>();
            foreach (string ds in dicestrings)
            {
                string[] dicesplit = ds.ToLower().Split('d');
                if (dicesplit.Length != 2)
                {
                    Badroll(); return;
                }
                try
                {
                    string[] dmsplit = dicesplit[1].Split('+', '-'); //check for modifier.
                    if (dmsplit.Length == 1)
                    {
                        dicelist.Add((int.Parse(dicesplit[0]), int.Parse(dicesplit[1]), 0));

                        continue;
                    }
                    dicelist.Add((int.Parse(dicesplit[0]), int.Parse(dmsplit[0]), int.Parse(dmsplit[1]) * (dicesplit[1].Contains('+') ? 1 : -1)));

                }
                catch (FormatException)
                {
                    Badroll(); return;
                }
                catch (OverflowException)
                {
                    await ctx.RespondAsync($"I am sorry, but my positronic brain cant handle numbers bigger than {int.MaxValue}.");
                    return;
                }

            }
            try
            {
                if (dicelist.Aggregate(0, (c, n) => checked(c + n.dc)) > 100)
                {
                    await ctx.RespondAsync("Please limit yourself to 100 dice rolls per command. my hands get sore.");
                    return;
                }
            }
            catch (OverflowException)
            {
                await ctx.RespondAsync("Please dont try to roll more dice than fit in an integer. 100 is plenty.");
                return;
            }


            try
            {
                BigInteger total = 0;
                response.Append($"Rolling {dicelist.Aggregate("", (c, n) => c + DieString(n) + ' ')}\n```");
                foreach ((int dc, int dn, int dm) die in dicelist)
                {
                    response.Append($"{DieString(die)}:\n");
                    for (int i = 1; i < die.dc + 1; i++)
                    {
                        long roll = _rand.Next(1, die.dn + 1);
                        response.Append($"{i}:{roll}{DmString(die)}\t");
                        total += roll + (long)die.dm;
                        if (i % 10 == 0) response.AppendLine();
                    }
                    response.AppendLine();
                }
                response.Append($"total:{total}```");
                if (response.Length > 1999)
                {
                    await ctx.RespondAsync(
                        $"I cant give you a detail report on that as it wont fit in a message, but your total was: {total}");
                    return;
                }
                await ctx.RespondAsync(response.ToString());

            }
            catch (ArgumentException e)
            {

            }
            catch (Exception e)
            {
                Badroll();
                return;
            }

            string DieString((int dc, int dn, int dm) die)
            {

                return $"{die.dc}d{die.dn}" + DmString(die);
            }

            string DmString((int dc, int dn, int dm) die)
            {
                if (die.dm == 0)
                    return $"";
                if (die.dm > 0)
                    return $"+{die.dm}";

                return $"{ die.dm }";
            }

            async void Badroll()
            {
                await ctx.RespondAsync("I did not understand your roll command, use ```!roll #d#[(+|-)#] [...]```\n Eg !roll 10d5 3d4-2");
            }
        }


        [Command("status")]
        [Description("Status report.")]
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

        [Command("move")]
        public async Task Move(CommandContext ctx, DiscordMember user, DiscordChannel target)
        {
            await user.ModifyAsync(null, null, null, null, target, "for the lulz.");
        }


    }
}
