namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShipmentId_OrderProduct_Table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "ShipmentId", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "ShipmentId");
        }
    }
}
