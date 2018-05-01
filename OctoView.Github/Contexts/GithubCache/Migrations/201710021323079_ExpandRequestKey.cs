namespace GithubDashboard.Contexts.GithubCache.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandRequestKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.GithubRequests");
            AlterColumn("dbo.GithubRequests", "Key", c => c.String(nullable: false, maxLength: 450));
            AddPrimaryKey("dbo.GithubRequests", "Key");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.GithubRequests");
            AlterColumn("dbo.GithubRequests", "Key", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.GithubRequests", "Key");
        }
    }
}
