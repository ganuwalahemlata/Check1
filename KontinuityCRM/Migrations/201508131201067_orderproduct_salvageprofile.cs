namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderproduct_salvageprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "SalvageProfileId", c => c.Int());
            CreateIndex("dbo.OrderProducts", "SalvageProfileId");
            AddForeignKey("dbo.OrderProducts", "SalvageProfileId", "dbo.SalvageProfiles", "Id");
            DropColumn("dbo.OrderProducts", "PrepaidIncrement");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProducts", "PrepaidIncrement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.OrderProducts", "SalvageProfileId", "dbo.SalvageProfiles");
            DropIndex("dbo.OrderProducts", new[] { "SalvageProfileId" });
            DropColumn("dbo.OrderProducts", "SalvageProfileId");
        }
    }
}
