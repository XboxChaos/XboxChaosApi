namespace XboxChaosApi.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedBasicReleasemodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Releases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ReleaseMode = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Releases");
        }
    }
}
