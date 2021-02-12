using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace ShitpostTron5000.Data
{
    public class Sailor
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public ulong SailorDiscordUserId { get; set; }
    }
}
