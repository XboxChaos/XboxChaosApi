namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createddb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationBranches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Ref = c.String(nullable: false),
                        RepoTree = c.String(nullable: false),
                        BuildDownload = c.String(),
                        UpdaterDownload = c.String(),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        Application_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applications", t => t.Application_Id)
                .Index(t => t.Application_Id);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        RepoName = c.String(nullable: false),
                        RepoUrl = c.String(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationBranches", "Application_Id", "dbo.Applications");
            DropIndex("dbo.ApplicationBranches", new[] { "Application_Id" });
            DropTable("dbo.Applications");
            DropTable("dbo.ApplicationBranches");
        }
    }
}
