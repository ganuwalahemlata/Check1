namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_rebillreportdisplay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "RebillReportDisplay", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "RebillReportDisplay");
        }
    }
}
