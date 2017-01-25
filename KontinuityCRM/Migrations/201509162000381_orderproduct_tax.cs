namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproduct_tax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "Tax");
        }
    }
}
