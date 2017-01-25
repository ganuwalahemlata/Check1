namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile_addstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Status", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "Status");
        }
    }
}
