namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NextSalvageProfileId_in_OP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "NextSalvageProfileId", c => c.Int());
            CreateIndex("dbo.OrderProducts", "NextSalvageProfileId");
            AddForeignKey("dbo.OrderProducts", "NextSalvageProfileId", "dbo.SalvageProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProducts", "NextSalvageProfileId", "dbo.SalvageProfiles");
            DropIndex("dbo.OrderProducts", new[] { "NextSalvageProfileId" });
            DropColumn("dbo.OrderProducts", "NextSalvageProfileId");
        }
    }
}
