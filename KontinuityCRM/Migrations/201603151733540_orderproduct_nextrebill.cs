namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproduct_nextrebill : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "NextRebillPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.OrderProducts", "RebillFrequency", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "RebillFrequency");
            DropColumn("dbo.OrderProducts", "NextRebillPrice");
        }
    }
}
