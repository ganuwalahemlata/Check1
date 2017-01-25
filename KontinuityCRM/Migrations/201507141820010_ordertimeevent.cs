namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ordertimeevent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderTimeEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(),
                        Time = c.DateTimeOffset(nullable: false, precision: 7),
                        Event = c.Byte(nullable: false),
                        Action = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId)
                .Index(t => t.Event)
                .Index(t => t.Action);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderTimeEvents", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderTimeEvents", "OrderId", "dbo.Orders");
            DropIndex("dbo.OrderTimeEvents", new[] { "Action" });
            DropIndex("dbo.OrderTimeEvents", new[] { "Event" });
            DropIndex("dbo.OrderTimeEvents", new[] { "ProductId" });
            DropIndex("dbo.OrderTimeEvents", new[] { "OrderId" });
            DropTable("dbo.OrderTimeEvents");
        }
    }
}
