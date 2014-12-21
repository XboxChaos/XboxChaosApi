namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBuildDownloadstring : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Releases", "BuildDownload", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Releases", "BuildDownload");
        }
    }
}
