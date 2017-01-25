namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSinglePurchaseLimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsSinglePurchaseLimit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsSinglePurchaseLimit");
        }
    }
}
