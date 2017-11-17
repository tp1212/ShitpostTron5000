using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace ShitpostTron5000
{
    class Archiver
    {

        public DiscordGuildGetter Guild { get; set; }
        public List<DiscordChannelGetter> Channels { get; set; }


        public async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Guild != Guild.GetDiscordEntity())
                return;
            if (Channels.All(x => x.GetDiscordEntity() != e.Channel))
                return;
            MessageArchiveEntry msg = new MessageArchiveEntry();
            msg.CurrentMessage = new DiscordMessageGetter(){SnowFlakeUlong = e.Message.Id};
            msg.Content = e.Message.Content;
            msg.UserName = e.Message.Author.Username;


        }
    }
}
