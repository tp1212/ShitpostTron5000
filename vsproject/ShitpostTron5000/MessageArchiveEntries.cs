using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace ShitpostTron5000
{
    class MessageArchiveEntry
    {
        [Key]
        public long Id { get; set; }


        public DiscordMessageGetter CurrentMessage { get; set; }
        public MessageArchiveEntry PreviousVersion { get; set; }

        public string Content { get; set; }
        public string UserName { get; set; }
        public ulong UserId { get; set; }
        [NotMapped]//Mysql is a giant dumpster fire.
        public DateTimeOffset Posted {
            get => new DateTimeOffset(PostedDateTime,PostedOffset);
            set { PostedDateTime = value.DateTime;
                PostedOffset = value.Offset;
            }
        }

        public DateTime PostedDateTime { get; set; }
        public TimeSpan PostedOffset { get; set; }
        public DiscordChannelGetter Channel { get; set; }
    }
}
