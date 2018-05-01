namespace GithubDashboard.Contexts.GithubCache.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGithubRequestCache : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GithubRequests",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Request = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GithubRequests");
        }
    }
}
