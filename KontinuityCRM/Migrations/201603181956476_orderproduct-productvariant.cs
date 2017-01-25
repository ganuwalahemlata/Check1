namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproductproductvariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "ProductVariantId", c => c.Int(nullable: true));
            CreateIndex("dbo.OrderProducts", "ProductVariantId");
            AddForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants", "ProductVariantId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProducts", "ProductVariantId", "dbo.ProductVariants");
            DropIndex("dbo.OrderProducts", new[] { "ProductVariantId" });
            DropColumn("dbo.OrderProducts", "ProductVariantId");
        }
    }
}
