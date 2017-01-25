namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalvageProfileChaining_Diff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalvageProfiles", "NextSavageProfile", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalvageProfiles", "NextSavageProfile");
        }
    }
}
