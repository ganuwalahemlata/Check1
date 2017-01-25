namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ordertimeevent_subid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderTimeEvents", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderTimeEvents", new[] { "ProductId" });
            AddColumn("dbo.OrderTimeEvents", "AffiliateId", c => c.String());
            AddColumn("dbo.OrderTimeEvents", "SubId", c => c.String());
            AlterColumn("dbo.Partials", "AffiliateId", c => c.String());
            AlterColumn("dbo.Partials", "SubId", c => c.String());
            AlterColumn("dbo.Orders", "AffiliateId", c => c.String());
            AlterColumn("dbo.Orders", "SubId", c => c.String());
            AlterColumn("dbo.OrderTimeEvents", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderTimeEvents", "ProductId");
            AddForeignKey("dbo.OrderTimeEvents", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderTimeEvents", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderTimeEvents", new[] { "ProductId" });
            AlterColumn("dbo.OrderTimeEvents", "ProductId", c => c.Int());
            AlterColumn("dbo.Orders", "SubId", c => c.Int());
            AlterColumn("dbo.Orders", "AffiliateId", c => c.Int());
            AlterColumn("dbo.Partials", "SubId", c => c.Int());
            AlterColumn("dbo.Partials", "AffiliateId", c => c.Int());
            DropColumn("dbo.OrderTimeEvents", "SubId");
            DropColumn("dbo.OrderTimeEvents", "AffiliateId");
            CreateIndex("dbo.OrderTimeEvents", "ProductId");
            AddForeignKey("dbo.OrderTimeEvents", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
