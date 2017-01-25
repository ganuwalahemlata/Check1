namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billvalue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "BillValue", c => c.Int());
            AlterColumn("dbo.SalvageProfiles", "BillValue", c => c.Int(nullable: false));
            DropColumn("dbo.SalvageProfiles", "LowerPercent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalvageProfiles", "LowerPercent", c => c.Int());
            AlterColumn("dbo.SalvageProfiles", "BillValue", c => c.String());
            AlterColumn("dbo.Products", "BillValue", c => c.String());
        }
    }
}
