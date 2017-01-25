namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_displays3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserProfile", "ObjectLogDisplay");
            DropColumn("dbo.UserProfile", "ViewLogDisplay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "ViewLogDisplay", c => c.Int(nullable: false));
            AddColumn("dbo.UserProfile", "ObjectLogDisplay", c => c.Int(nullable: false));
        }
    }
}
