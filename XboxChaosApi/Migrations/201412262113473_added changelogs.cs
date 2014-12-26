namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedchangelogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationBranches", "Changelog", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationBranches", "Changelog");
        }
    }
}
