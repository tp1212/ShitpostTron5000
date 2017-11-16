namespace ShitpostTron5000.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class messageachrive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageArchiveEntries",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PreviousVersion_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MessageArchiveEntries", t => t.PreviousVersion_Id)
                .Index(t => t.PreviousVersion_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageArchiveEntries", "PreviousVersion_Id", "dbo.MessageArchiveEntries");
            DropIndex("dbo.MessageArchiveEntries", new[] { "PreviousVersion_Id" });
            DropTable("dbo.MessageArchiveEntries");
        }
    }
}
