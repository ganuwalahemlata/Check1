namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserGroupId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "UserGroupId", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.UserProfile", "UserGroupId");
        }
    }
}
