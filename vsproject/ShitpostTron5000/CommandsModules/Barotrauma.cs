using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Serilog;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{
    class Barotrauma
    {
        [Command("Barotrauma")][Description("Start registration for a round of Barotrauma")]
        public async Task barotraumaRegister(CommandContext ctx)
        {
            ShitpostTronContext db = Program.GetDbContext();
            db.Sailors.RemoveRange(db.Sailors); // Clear the table of sailors 

            // Post a message along with a reaction
            var message = await ctx.RespondAsync("Avast mateys, it be sailin' time");
            const string emj = "⚓";
            var discordClient = Program.Client;
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode(discordClient, emj));

            // Wait for reactions - add a sailor for every person who reacts
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

                Sailor s = new Sailor
                {
                    Role = "",
                    SailorDiscordUserId = reacts.User.Id
                };
                db.Add(s);
                await db.SaveChangesAsync();
            }
        }

        [Command("Sail")][Description("Assign roles for Barotrauma")]
        public async Task barotraumaLaunch(CommandContext context){ //TODO Vanni this is an awful mess please clean it up

            ShitpostTronContext db = Program.GetDbContext();
            var discordClient = Program.Client;

            // Get the list of roles from the roles.csv file. 
            List<(string role, string missionText)> roles = File.ReadAllLines("./data/barotraumaroles.csv")
                .Select(x => x.Split(';'))
                .Select(x=>(x[0],x[1]))
                .ToList();
            

            var traitorPercentage = 0.2; //TODO move to a config file
            var traitorDuplicateRoles = true; //TODO implement false

            List<Sailor> sailors = db.Sailors.ToList(); // Get list of registered sailors from the database
            var numTraitors = Math.Floor( (sailors.Count - 1) * traitorPercentage); // Calculate the amount of traitors based on the amount of sailors

            Random rand = new Random();
            

            for(int i = 0; i < numTraitors; i++){
                var role = roles[rand.Next(roles.Count)]; // Random role
                var sailor = sailors[rand.Next(sailors.Count)]; // Random sailor
                sailors.Remove(sailor); // Remove sailor from eligibles list

                // Tell the traitor(s) what to do

                var user = await discordClient.GetUserAsync(sailor.SailorDiscordUserId);
                var dm = await discordClient.CreateDmAsync(user);
                await dm.SendMessageAsync("Congratulations! You have been selected to be Bad today");
                await dm.SendMessageAsync(role.role);
                await dm.SendMessageAsync(role.missionText);
            }

            // Message remaining sailors that they are not the guy
            foreach (var sailor in sailors) {
                var user = await discordClient.GetUserAsync(sailor.SailorDiscordUserId);
                var dm = await discordClient.CreateDmAsync(user);
                await dm.SendMessageAsync("You are not the guy");
            }

        }
    }
}