using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;

namespace ShitpostTron5000
{
    public class ExpanderChannel
    {
      
        

        
        public int Id { get; set; }

        public DiscordChannelGetter Category { get; set; }

        public DiscordGuildGetter Guild { get; set; }

        public string BaseName { get; set; }

        public static async Task<ExpanderChannel> BuildExpanderChannel(DiscordGuild targetServer, string catname, string channelname)
        {

            DiscordChannel cat = await targetServer.CreateChannelAsync(catname, ChannelType.Category);
            DiscordChannel first = await targetServer.CreateChannelAsync(channelname + 1, ChannelType.Voice, cat);
         
            return new ExpanderChannel(targetServer,channelname,cat);
        }

        public ExpanderChannel()
        {
        }

        public ExpanderChannel(DiscordGuild targetGuild, string baseName, DiscordChannel category)
        {
            Guild = new DiscordGuildGetter {SnowFlakeUlong = targetGuild.Id};
            Category = new DiscordChannelGetter { SnowFlakeUlong = category.Id };
            this.BaseName = baseName;
        }

        public async Task OnChannelUpdate(VoiceStateUpdateEventArgs e)
        {
            try
            {

            if (e.Guild != await Guild.GetDiscordEntityAsync()) 
                return;//not my problem
            if (e.Channel?.Parent != await Category.GetDiscordEntityAsync())
                return;//also my problem
            if (await Category.GetDiscordEntityAsync() == null)//somethings wrong.
                return;


            lock (this)
            {
                
                if (_isUpdating)
                {
                    return; //dont continue stomping over the other tasks changes.
                }
                _isUpdating = true; //prevent other tasks from stomping over my changes.
            }

                List<DiscordMember> members =  (await Guild.GetDiscordEntityAsync()).Members.ToList();
               
                List<DiscordChannel> expanderChannels = (await Category.GetDiscordEntityAsync()).Children.ToList();
                IEnumerable<DiscordChannel> empty =
                    expanderChannels.Where(x => members.All(u => u.VoiceState?.Channel != x))
                        .ToList(); //find empty channels

                if (!empty.Any()) //all channels full
                {
                    //add new channel
                    await (await Guild.GetDiscordEntityAsync()).CreateChannelAsync(BaseName + (expanderChannels.Count + 1), ChannelType.Voice,
                        (await Category.GetDiscordEntityAsync()));
                }

                foreach (DiscordChannel discordChannel in empty.Skip(1)) //some channels are empty. 
                {
                    await discordChannel.DeleteAsync("bot auto deleting empty expander channel");
                    expanderChannels.Remove(discordChannel);
                }

                if (empty.Any()) // move empty channels to end of list.
                {
                  expanderChannels = expanderChannels.OrderBy(x => empty.Any(y => x == y)).ToList();
                }
                //restructure names.
                int i = 1;
                // ReSharper disable once LoopCanBeConvertedToQuery code clarity
                foreach (DiscordChannel channl in expanderChannels)
                {
                    try
                    {
                        await channl.ModifyAsync(BaseName + i, i++);
                    }
                    catch (NotFoundException)
                    {
                        Console.WriteLine("Tried to modify a non existant channel.");
       
                    }
                    
                }

            }
            finally
            {
                _isUpdating = false;
            }
        }
        
        private volatile bool _isUpdating = false;
    }
}
