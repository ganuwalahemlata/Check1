namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_apikey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "APIKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "APIKey");
        }
    }
}
