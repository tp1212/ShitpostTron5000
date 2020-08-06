using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace ShitpostTron5000.Data
{
    public class Quote
    {
        public int Id { get; set; }
        public string QuoteText { get; set; }
        public ulong DiscordSnowFlake { get; set; }
        public string QuoteeName { get; set; }
        public ulong QouteeDiscordSnowflake { get; set; }
    }
}
