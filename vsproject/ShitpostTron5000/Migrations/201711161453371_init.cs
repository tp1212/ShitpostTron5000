namespace ShitpostTron5000.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
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
                "dbo.DbTests",
                c => new
                    {
                        DbTestId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Life = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.DbTestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpanderChannels", "Guild_Id", "dbo.DiscordGuildGetters");
            DropForeignKey("dbo.ExpanderChannels", "Category_Id", "dbo.DiscordChannelGetters");
            DropIndex("dbo.ExpanderChannels", new[] { "Guild_Id" });
            DropIndex("dbo.ExpanderChannels", new[] { "Category_Id" });
            DropTable("dbo.DbTests");
            DropTable("dbo.DiscordGuildGetters");
            DropTable("dbo.DiscordChannelGetters");
            DropTable("dbo.ExpanderChannels");
        }
    }
}
