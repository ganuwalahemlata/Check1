namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserGroups", "Status", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserGroups", "Status");
        }
    }
}
