using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace ShitpostTron5000
{
    class BigBrother
    {
        public int Id { get; set; }

        public DiscordGuildGetter Guild { get; set; }
        public List<DiscordChannelGetter> ChannelBlacklist { get; set; }
        public async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Guild != Guild.GetDiscordEntity())//Not a guild that we are monitoring
                return;
            if (ChannelBlacklist.Any(x => x.GetDiscordEntity() == e.Channel))//channel is in blacklist, so ignore this message.
                return;
            MessageArchiveEntry msg = new MessageArchiveEntry
            {
                CurrentMessage = (DiscordMessageGetter) e.Message,
                Channel = (DiscordChannelGetter) e.Channel,
                Posted = e.Message.Timestamp,
                Content = e.Message.Content,
                UserName = e.Message.Author.Username,
                UserId = e.Message.Author.Id
            };
            Program.ShitpostTronContext.MessageArchive.Add(msg);
        }

        public void OnMessageEdited()
        {
            
        }
    }
}
