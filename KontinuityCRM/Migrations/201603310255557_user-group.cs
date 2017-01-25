namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usergroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGroupsInRoles",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        Role = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GroupId, t.Role })
                .ForeignKey("dbo.UserGroups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroupsInRoles", "GroupId", "dbo.UserGroups");
            DropIndex("dbo.UserGroupsInRoles", new[] { "GroupId" });
            DropTable("dbo.UserGroupsInRoles");
        }
    }
}
