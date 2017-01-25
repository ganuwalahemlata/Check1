namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalvageProfileChaining_Diff2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalvageProfiles", "NextSalvageProfile_Id", c => c.Int());
            AddForeignKey("dbo.SalvageProfiles", "NextSalvageProfile_Id", "dbo.SalvageProfiles", "Id");
            CreateIndex("dbo.SalvageProfiles", "NextSalvageProfile_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalvageProfiles", "NextSalvageProfile_Id", "dbo.SalvageProfiles");
            DropIndex("dbo.SalvageProfiles", new[] { "NextSalvageProfile_Id" });
            DropColumn("dbo.SalvageProfiles", "NextSalvageProfile_Id");
        }
    }
}
