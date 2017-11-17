using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitpostTron5000
{
    class MessageArchiveEntry
    {
        public long Id { get; set; }
        public DiscordMessageGetter CurrentMessage;
        public MessageArchiveEntry PreviousVersion { get; set; }

        public string Content { get; set; }
        public string UserName { get; set; }


    }
}
