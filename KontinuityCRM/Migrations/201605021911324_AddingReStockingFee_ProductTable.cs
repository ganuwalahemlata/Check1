namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingReStockingFee_ProductTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ReStockingFee", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ReStockingFee");
        }
    }
}
