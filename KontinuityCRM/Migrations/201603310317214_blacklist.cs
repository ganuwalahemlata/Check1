namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blacklist : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlackLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreditCard = c.String(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        LastUsedDate = c.DateTime(),
                        Attempts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserProfile", "BlackListDisplay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "BlackListDisplay");
            DropTable("dbo.BlackLists");
        }
    }
}
