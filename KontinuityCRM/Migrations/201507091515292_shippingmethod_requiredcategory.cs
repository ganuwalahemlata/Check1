namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shippingmethod_requiredcategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShippingMethods", "ShippingCategoryId", "dbo.ShippingCategories");
            DropIndex("dbo.ShippingMethods", new[] { "ShippingCategoryId" });
            AlterColumn("dbo.ShippingMethods", "ShippingCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShippingMethods", "ShippingCategoryId");
            AddForeignKey("dbo.ShippingMethods", "ShippingCategoryId", "dbo.ShippingCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShippingMethods", "ShippingCategoryId", "dbo.ShippingCategories");
            DropIndex("dbo.ShippingMethods", new[] { "ShippingCategoryId" });
            AlterColumn("dbo.ShippingMethods", "ShippingCategoryId", c => c.Int());
            CreateIndex("dbo.ShippingMethods", "ShippingCategoryId");
            AddForeignKey("dbo.ShippingMethods", "ShippingCategoryId", "dbo.ShippingCategories", "Id");
        }
    }
}
