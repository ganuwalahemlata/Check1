namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproductproductvariantdelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants");
            DropIndex("dbo.OrderProducts", new[] { "ProductVariantId" });
            AlterColumn("dbo.OrderProducts", "ProductVariantId", c => c.Int());
            CreateIndex("dbo.OrderProducts", "ProductVariantId");
            AddForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants", "ProductVariantId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants");
            DropIndex("dbo.OrderProducts", new[] { "ProductVariantId" });
            AlterColumn("dbo.OrderProducts", "ProductVariantId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderProducts", "ProductVariantId");
            AddForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants", "ProductVariantId", cascadeDelete: true);
        }
    }
}
