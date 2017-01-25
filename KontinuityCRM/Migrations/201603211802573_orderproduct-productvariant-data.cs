namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproductproductvariantdata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "SKU", c => c.String());
            AddColumn("dbo.OrderProducts", "Cost", c => c.Double(nullable: false));
            AddColumn("dbo.OrderProducts", "Currency", c => c.Int(nullable: false));
            AddColumn("dbo.OrderProducts", "FieldName", c => c.String());
            AddColumn("dbo.OrderProducts", "FieldValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "FieldValue");
            DropColumn("dbo.OrderProducts", "FieldName");
            DropColumn("dbo.OrderProducts", "Currency");
            DropColumn("dbo.OrderProducts", "Cost");
            DropColumn("dbo.OrderProducts", "SKU");
        }
    }
}
