namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproductdeleterebillfields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderProducts", "NextRebillPrice");
            DropColumn("dbo.OrderProducts", "RebillFrequency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProducts", "RebillFrequency", c => c.Int());
            AddColumn("dbo.OrderProducts", "NextRebillPrice", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
