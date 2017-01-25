namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LowerPercentage_SalvageProofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalvageProfiles", "LowerPercentage", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalvageProfiles", "LowerPercentage");
        }
    }
}
