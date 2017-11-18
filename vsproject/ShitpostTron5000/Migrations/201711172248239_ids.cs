namespace ShitpostTron5000.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ids : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BigBrothers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guild_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DiscordGuildGetters", t => t.Guild_Id)
                .Index(t => t.Guild_Id);
            
            AddColumn("dbo.DiscordChannelGetters", "BigBrother_Id", c => c.Int());
            AddColumn("dbo.MessageArchiveEntries", "PostedDateTime", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.MessageArchiveEntries", "Channel_Id", c => c.Int());
            CreateIndex("dbo.DiscordChannelGetters", "BigBrother_Id");
            CreateIndex("dbo.MessageArchiveEntries", "Channel_Id");
            AddForeignKey("dbo.MessageArchiveEntries", "Channel_Id", "dbo.DiscordChannelGetters", "Id");
            AddForeignKey("dbo.DiscordChannelGetters", "BigBrother_Id", "dbo.BigBrothers", "Id");
            DropColumn("dbo.MessageArchiveEntries", "PostedDT");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MessageArchiveEntries", "PostedDT", c => c.DateTime(nullable: false, precision: 0));
            DropForeignKey("dbo.BigBrothers", "Guild_Id", "dbo.DiscordGuildGetters");
            DropForeignKey("dbo.DiscordChannelGetters", "BigBrother_Id", "dbo.BigBrothers");
            DropForeignKey("dbo.MessageArchiveEntries", "Channel_Id", "dbo.DiscordChannelGetters");
            DropIndex("dbo.BigBrothers", new[] { "Guild_Id" });
            DropIndex("dbo.MessageArchiveEntries", new[] { "Channel_Id" });
            DropIndex("dbo.DiscordChannelGetters", new[] { "BigBrother_Id" });
            DropColumn("dbo.MessageArchiveEntries", "Channel_Id");
            DropColumn("dbo.MessageArchiveEntries", "PostedDateTime");
            DropColumn("dbo.DiscordChannelGetters", "BigBrother_Id");
            DropTable("dbo.BigBrothers");
        }
    }
}
