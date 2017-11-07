using System;
using System.Data.Entity;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace ShitpostTron5000
{
    class Program
    {
        public static DiscordClient Client;
        static CommandsNextModule _commands;
        public static DateTime Start;
        public static DbSet<ExpanderChannel> ExpanderChannels;


        static void Main(string[] args)
        {
            Start = DateTime.Now;
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
           
            while (true)
            {
               string msg = Console.ReadLine();
                Client.SendMessageAsync( Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), msg);
            }
        }

        static async Task init(string[] args)
        {
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Token.TokenStr,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });
            
            ShitpostTronContext db = new ShitpostTronContext();
            ExpanderChannels = db.ExpanderChannels;
            ExpanderChannels.Local.CollectionChanged += async (sender, eventArgs) => { await db.SaveChangesAsync(); };
            foreach (ExpanderChannel expanderChannel in ExpanderChannels)
            {
               Client.VoiceStateUpdated+= expanderChannel.OnChannelUpdate;
            }
        }
        

            static async Task MainAsync(string[] args)
            {

                await init(args);
            
             _commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            _commands.RegisterCommands<BasicCommands>();
            
            _commands.CommandErrored += async e =>
            {
               Console.WriteLine(  e.Exception.ToString());
            };
           
            Client.MessageCreated += async e =>
            {
                if(e.Channel.Name == "devtrons")
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

          
            Client.VoiceStateUpdated += async e =>
            {
                
                //await Client.SendMessageAsync(Client.GetChannelAsync(245227159445045249).GetAwaiter().GetResult(), $"{e.User.Username} just joined {e.Channel.Name}");
            };

            

            

            await Client.ConnectAsync();
           
        }
    }
}


