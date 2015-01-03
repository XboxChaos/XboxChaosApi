namespace XboxChaosApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChangeLink : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Changelogs", name: "ApplicationBranch_Id", newName: "Branch_Id");
            RenameIndex(table: "dbo.Changelogs", name: "IX_ApplicationBranch_Id", newName: "IX_Branch_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Changelogs", name: "IX_Branch_Id", newName: "IX_ApplicationBranch_Id");
            RenameColumn(table: "dbo.Changelogs", name: "Branch_Id", newName: "ApplicationBranch_Id");
        }
    }
}
