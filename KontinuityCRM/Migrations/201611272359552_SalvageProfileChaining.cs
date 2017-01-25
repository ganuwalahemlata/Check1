namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalvageProfileChaining : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalvageProfiles", "NextSalvageProfile", c => c.Int());
            AddColumn("dbo.SalvageProfiles", "EnableDiscount", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalvageProfiles", "AfterDecline", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalvageProfiles", "AfterApprove", c => c.Boolean(nullable: false));
            AddColumn("dbo.SalvageProfiles", "NextSavageProfile", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalvageProfiles", "NextSavageProfile");
            DropColumn("dbo.SalvageProfiles", "AfterApprove");
            DropColumn("dbo.SalvageProfiles", "AfterDecline");
            DropColumn("dbo.SalvageProfiles", "EnableDiscount");
            DropColumn("dbo.SalvageProfiles", "NextSalvageProfile");
        }
    }
}
