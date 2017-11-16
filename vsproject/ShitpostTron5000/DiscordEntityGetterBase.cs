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
    public abstract class DiscordEntityGetterBase<T> where T : SnowflakeObject
    {
        protected ulong SnowFlake;

        [Key]
        public int Id { get; set; }

        public long SnowFlakeLong
        {

            get => (long) SnowFlake;
            set => SnowFlake = (ulong) value;
        }
        [NotMapped]
        public ulong SnowFlakeUlong
        {

            get => SnowFlake;
            set => SnowFlake = value;
        }


        private T _entity;

        protected abstract Task<T> GetDiscordEntityInternal();

        public async Task<T> GetDiscordEntity()
        {
             return _entity ?? (_entity=  await GetDiscordEntityInternal() );
        }


    }


    public class DiscordGuildGetter : DiscordEntityGetterBase<DiscordGuild>
    {
        protected override async Task<DiscordGuild> GetDiscordEntityInternal()
        {
          return await Program.Client.GetGuildAsync(SnowFlake);
        }
    }

    public class DiscordChannelGetter : DiscordEntityGetterBase<DiscordChannel>
    {
        protected override async Task<DiscordChannel> GetDiscordEntityInternal()
        {
          return await Program.Client.GetChannelAsync(SnowFlake);
        }
    }

    public class DiscordMessageGetter : DiscordEntityGetterBase<DiscordMessage>
    {

        [Key]
        public int SubId { get; set; }
        public DiscordChannelGetter Channel { get; set; }
        
        protected override async Task<DiscordMessage> GetDiscordEntityInternal()
        {
            return await (await Channel.GetDiscordEntity()).GetMessageAsync(SnowFlake);
        }
    }

}
