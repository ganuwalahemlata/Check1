namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_displays1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "DeclineReasonDisplay", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "DeclineReasonDisplay");
        }
    }
}
