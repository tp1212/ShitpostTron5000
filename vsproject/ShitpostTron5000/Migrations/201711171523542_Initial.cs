namespace ShitpostTron5000.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpanderChannels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseName = c.String(unicode: false),
                        Category_Id = c.Int(),
                        Guild_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscordChannelGetters", t => t.Category_Id)
                .ForeignKey("dbo.DiscordGuildGetters", t => t.Guild_Id)
                .Index(t => t.Category_Id)
                .Index(t => t.Guild_Id);
            
            CreateTable(
                "dbo.DiscordChannelGetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SnowFlakeLong = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiscordGuildGetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SnowFlakeLong = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageArchiveEntries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Content = c.String(unicode: false),
                        UserName = c.String(unicode: false),
                        PostedDT = c.DateTime(nullable: false, precision: 0),
                        PostedOffset = c.Time(nullable: false, precision: 0),
                        CurrentMessage_Id = c.Int(),
                        PreviousVersion_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscordMessageGetters", t => t.CurrentMessage_Id)
                .ForeignKey("dbo.MessageArchiveEntries", t => t.PreviousVersion_Id)
                .Index(t => t.CurrentMessage_Id)
                .Index(t => t.PreviousVersion_Id);
            
            CreateTable(
                "dbo.DiscordMessageGetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SnowFlakeLong = c.Long(nullable: false),
                        Channel_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscordChannelGetters", t => t.Channel_Id, cascadeDelete: true)
                .Index(t => t.Channel_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageArchiveEntries", "PreviousVersion_Id", "dbo.MessageArchiveEntries");
            DropForeignKey("dbo.MessageArchiveEntries", "CurrentMessage_Id", "dbo.DiscordMessageGetters");
            DropForeignKey("dbo.DiscordMessageGetters", "Channel_Id", "dbo.DiscordChannelGetters");
            DropForeignKey("dbo.ExpanderChannels", "Guild_Id", "dbo.DiscordGuildGetters");
            DropForeignKey("dbo.ExpanderChannels", "Category_Id", "dbo.DiscordChannelGetters");
            DropIndex("dbo.DiscordMessageGetters", new[] { "Channel_Id" });
            DropIndex("dbo.MessageArchiveEntries", new[] { "PreviousVersion_Id" });
            DropIndex("dbo.MessageArchiveEntries", new[] { "CurrentMessage_Id" });
            DropIndex("dbo.ExpanderChannels", new[] { "Guild_Id" });
            DropIndex("dbo.ExpanderChannels", new[] { "Category_Id" });
            DropTable("dbo.DiscordMessageGetters");
            DropTable("dbo.MessageArchiveEntries");
            DropTable("dbo.DiscordGuildGetters");
            DropTable("dbo.DiscordChannelGetters");
            DropTable("dbo.ExpanderChannels");
        }
    }
}
