using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;

namespace ShitpostTron5000.Data
{
    public class Sailor
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public DiscordUser SailorDiscordUser { get; set; }
    }
}
