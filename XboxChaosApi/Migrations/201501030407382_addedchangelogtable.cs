namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedchangelogtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Changelogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FriendlyVersion = c.String(nullable: false),
                        InternalVersion = c.String(nullable: false),
                        Changes = c.String(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ApplicationBranch_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationBranches", t => t.ApplicationBranch_Id)
                .Index(t => t.ApplicationBranch_Id);
            
            DropColumn("dbo.ApplicationBranches", "Changelog");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationBranches", "Changelog", c => c.String());
            DropForeignKey("dbo.Changelogs", "ApplicationBranch_Id", "dbo.ApplicationBranches");
            DropIndex("dbo.Changelogs", new[] { "ApplicationBranch_Id" });
            DropTable("dbo.Changelogs");
        }
    }
}
