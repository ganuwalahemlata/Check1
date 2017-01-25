namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testcardnumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestCardNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserProfile", "TestCardDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.Orders", "IsTest", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsTest");
            DropColumn("dbo.UserProfile", "TestCardDisplay");
            DropTable("dbo.TestCardNumbers");
        }
    }
}
