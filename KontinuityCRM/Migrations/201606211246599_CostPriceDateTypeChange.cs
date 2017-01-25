namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CostPriceDateTypeChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductVariants", "Cost", c => c.Decimal(true, 18, 2));
            AlterColumn("ProductVariants", "Price", c => c.Decimal(true, 18, 2));
            AlterColumn("OrderProducts", "Cost", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 0));
        }

        public override void Down()
        {
            Sql("UPDATE ProductVariants SET Cost = 0 WHERE Price IS NULL");
            AlterColumn("ProductVariants", "Cost", c => c.Double(false));

            Sql("UPDATE ProductVariants SET Price = 0 WHERE Price IS NULL");
            AlterColumn("ProductVariants", "Price", c => c.Double(false));

            AlterColumn("OrderProducts", "Cost", c => c.Double(false, defaultValue: 0));
        }
    }
}
