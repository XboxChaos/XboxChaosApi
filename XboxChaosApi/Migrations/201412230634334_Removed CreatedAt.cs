namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCreatedAt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Releases", "CreatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Releases", "CreatedAt", c => c.DateTime(nullable: false));
        }
    }
}
