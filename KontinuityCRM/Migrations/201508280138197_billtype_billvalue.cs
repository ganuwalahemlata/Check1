namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billtype_billvalue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "BillType", c => c.Int());
            AddColumn("dbo.OrderProducts", "BillValue", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "BillValue");
            DropColumn("dbo.OrderProducts", "BillType");
        }
    }
}
