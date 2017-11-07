using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace ShitpostTron5000
{
    public class ExpanderChannel
    {
        private ExpanderChannel()
        {
        }
      

        private DiscordGuild _server;
        private DiscordChannel _category;

        [Key]
        public int Id { get; set; }

        public long CatId { get=>unchecked((long)_category.Id); set => _category = Program.Client.GetChannelAsync(unchecked ((ulong)value)).GetAwaiter().GetResult(); }

        public DiscordGuild Server => _server ?? (_server = _category.Guild);

        public string BaseName { get; set; }

        public static async Task<ExpanderChannel> BuildExpanderChannel(DiscordGuild targetServer, string catname, string channelname)
        {

            DiscordChannel cat = await targetServer.CreateChannelAsync(catname, ChannelType.Category);
            DiscordChannel first = await targetServer.CreateChannelAsync(channelname + 1, ChannelType.Voice, cat);
         
            return new ExpanderChannel(targetServer,channelname,cat);
        }

        public ExpanderChannel(DiscordGuild targetGuild, string baseName, DiscordChannel category)
        {
            _server = targetGuild;
            this.BaseName = baseName;
            _category = category;
        }

        public async Task OnChannelUpdate(VoiceStateUpdateEventArgs e)
        {
            if(e.Guild!= Server) //not my problem
                return;
            List<DiscordMember> members = Server.Members.ToList();
            List<DiscordChannel> expanderChannels = _category.Children.ToList();
            IEnumerable<DiscordChannel> empty = expanderChannels.Where(x => members.All(u => u.VoiceState?.Channel != x)).ToList();//find empty channels

            if (!empty.Any())//all channels full
            {
                //add new channel
                 await Server.CreateChannelAsync(BaseName + expanderChannels.Count, ChannelType.Voice, _category);
            }
            
            foreach (DiscordChannel discordChannel in empty.Skip(1))//some channels are empty. 
            {
               await discordChannel.DeleteAsync("bot auto deleting empty expander channel");
            }

            //restructure names.
            int i = 1;
            // ReSharper disable once LoopCanBeConvertedToQuery code clarity
            foreach (DiscordChannel channl in expanderChannels)
            {
               await channl.ModifyAsync(BaseName + i, i++);
            }
        }
    }
}
