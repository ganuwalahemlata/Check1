namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prepaid_increment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalvageProfiles", "PrepaidIncrement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderProducts", "PrepaidIncrement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            RenameColumn("dbo.Orders", "Prepaid", "IsPrepaid");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Orders", "IsPrepaid", "Prepaid");
            DropColumn("dbo.OrderProducts", "PrepaidIncrement");
            DropColumn("dbo.SalvageProfiles", "PrepaidIncrement");
        }
    }
}
