namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpermission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        UserGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Permissions = c.Long(),
                        Permissions1 = c.Long(),
                        Permissions2 = c.Long(),
                    })
                .PrimaryKey(t => t.UserGroupId);            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProfile", "UserGroupId", "dbo.UserGroups");
            DropIndex("dbo.UserProfile", new[] { "UserGroupId" });
            DropColumn("dbo.UserProfile", "UserGroupId");
            DropTable("dbo.UserGroups");
        }
    }
}
