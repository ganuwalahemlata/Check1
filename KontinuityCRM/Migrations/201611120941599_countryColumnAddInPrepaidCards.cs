namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countryColumnAddInPrepaidCards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrepaidCards", "Country", c => c.String(nullable: false));
            DropColumn("dbo.Orders", "BIN");
            DropColumn("dbo.Orders", "CreditLastFourNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CreditLastFourNumber", c => c.String());
            AddColumn("dbo.Orders", "BIN", c => c.String());
            DropColumn("dbo.PrepaidCards", "Country");
        }
    }
}
