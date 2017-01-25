namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_displays2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "SmtpServerDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "EventDisplay", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "EventDisplay");
            DropColumn("dbo.UserProfile", "SmtpServerDisplay");
        }
    }
}
