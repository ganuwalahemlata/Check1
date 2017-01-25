namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_PaymentTypesAndOrderPaymentTypesTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPaymentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PaymentTypeId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PaymentTypes", t => t.PaymentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.PaymentTypeId);
            
            CreateTable(
                "dbo.PaymentTypes",
                c => new
                    {
                        PaymentTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPaymentTypes", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductPaymentTypes", "PaymentTypeId", "dbo.PaymentTypes");
            DropIndex("dbo.ProductPaymentTypes", new[] { "PaymentTypeId" });
            DropIndex("dbo.ProductPaymentTypes", new[] { "ProductId" });
            DropTable("dbo.PaymentTypes");
            DropTable("dbo.ProductPaymentTypes");
        }
    }
}
