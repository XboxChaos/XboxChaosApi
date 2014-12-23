namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Releases",
                c => new
                    {
                        Branch = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        ReleaseMode = c.Int(nullable: false),
                        BuildDownload = c.String(nullable: false),
                        UpdaterDownload = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Branch);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Releases");
        }
    }
}
