﻿using System;
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

        protected DiscordEntityGetterBase()
        {
            
        }

        protected DiscordEntityGetterBase(T snowflakeObject)
        {
            _entity = snowflakeObject;
            SnowFlake = snowflakeObject.Id;
        }

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

        public async Task<T> GetDiscordEntityAsync()
        {
             return _entity ?? (_entity=  await GetDiscordEntityInternal() );
        }

        public T GetDiscordEntity()
        {
           return _entity ?? (_entity =  GetDiscordEntityInternal().GetAwaiter().GetResult());
        }



    }


    public class DiscordGuildGetter : DiscordEntityGetterBase<DiscordGuild>
    {
        public DiscordGuildGetter()
        {
        }
        public DiscordGuildGetter(DiscordGuild other) : base(other)
        {
        }


        protected override async Task<DiscordGuild> GetDiscordEntityInternal()
        {
          return await Program.Client.GetGuildAsync(SnowFlake);
        }

        public static explicit operator DiscordGuildGetter(DiscordGuild other)
        {
            return new DiscordGuildGetter(other){SnowFlake = other.Id};
        }


    }

    public class DiscordChannelGetter : DiscordEntityGetterBase<DiscordChannel>
    {

        public DiscordChannelGetter()
        {
        }

        public DiscordChannelGetter(DiscordChannel cnl) : base(cnl)
        {
        }

        protected override async Task<DiscordChannel> GetDiscordEntityInternal()
        {
          return await Program.Client.GetChannelAsync(SnowFlake);
        }

        public static explicit operator DiscordChannelGetter(DiscordChannel other)
        {
            return new DiscordChannelGetter(other) { SnowFlake = other.Id };
        }
    }

    public class DiscordMessageGetter : DiscordEntityGetterBase<DiscordMessage>
    {
        public DiscordMessageGetter()
        {
        }

        public DiscordMessageGetter(DiscordMessage msg ) : base(msg)
        {
            Channel = (DiscordChannelGetter) msg.Channel;
        }

        [Required]
        public DiscordChannelGetter Channel { get; set; }
        
        protected override async Task<DiscordMessage> GetDiscordEntityInternal()
        {
            return await (await Channel.GetDiscordEntityAsync()).GetMessageAsync(SnowFlake);
        }

        public static explicit operator DiscordMessageGetter(DiscordMessage other)
        {
            return new DiscordMessageGetter(other) { SnowFlake = other.Id };
        }
    }

}
