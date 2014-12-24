namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedbuildnames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationBranches", "FriendlyVersion", c => c.String());
            AddColumn("dbo.ApplicationBranches", "InternalVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationBranches", "InternalVersion");
            DropColumn("dbo.ApplicationBranches", "FriendlyVersion");
        }
    }
}
