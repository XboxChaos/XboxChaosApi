namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VersionNumbersAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Releases", "FriendlyVersion", c => c.String(nullable: false));
            AddColumn("dbo.Releases", "InternalVersion", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Releases", "InternalVersion");
            DropColumn("dbo.Releases", "FriendlyVersion");
        }
    }
}
