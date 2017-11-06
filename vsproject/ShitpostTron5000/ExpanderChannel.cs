using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Blehgh
{
    class ExpanderChannel
    {
       //List< DiscordChannel > _channels = new List<DiscordChannel>();
        private DiscordGuild _server;
        private DiscordChannel _category;
        private string _baseName;

        public static async Task<ExpanderChannel> BuildExpanderChannel(DiscordGuild targetServer, string catname, string channelname)
        {

            DiscordChannel cat = await targetServer.CreateChannelAsync(catname, ChannelType.Category);
            DiscordChannel first = await targetServer.CreateChannelAsync(channelname + 1, ChannelType.Voice, cat);
         
            return new ExpanderChannel(targetServer,channelname,cat);
        }

        public ExpanderChannel(DiscordGuild targetGuild, string BaseName, DiscordChannel category)
        {
            _server = targetGuild;
            _baseName = BaseName;
            _category = category;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public async Task OnChannelUpdate(VoiceStateUpdateEventArgs e)
        {
            if(e.Guild!= _server) //not my problem
                return;
            List<DiscordMember> members = _server.Members.ToList();
            List<DiscordChannel> expanderChannels = _category.Children.ToList();
            IEnumerable<DiscordChannel> empty = expanderChannels.Where(x => members.All(u => u.VoiceState?.Channel != x)).ToList();//find empty channels

            if (!empty.Any())//all channels full
            {
                //add new channel
                 await _server.CreateChannelAsync(_baseName + (expanderChannels.Count+1), ChannelType.Voice, _category);
            }
            
            foreach (DiscordChannel discordChannel in empty.Skip(1))//some channels are empty. 
            {
               await discordChannel.DeleteAsync("bot auto deleting empty expander channel");
            }

            expanderChannels = expanderChannels.OrderBy(x => empty.Any(y => y == x)).ToList();//ensure empty channels end up at the bottom.
            //restructure names.
            int i = 1;
            foreach (DiscordChannel channl in expanderChannels)
            { 
               await channl.ModifyAsync(_baseName + i, i++);
            }
        }
    }
}
