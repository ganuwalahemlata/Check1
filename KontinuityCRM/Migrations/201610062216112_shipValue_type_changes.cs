namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shipValue_type_changes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "ShipValue", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "ShipValue", c => c.Int());
        }
    }
}
