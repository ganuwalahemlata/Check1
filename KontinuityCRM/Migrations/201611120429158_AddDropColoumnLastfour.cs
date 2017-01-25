namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDropColoumnLastfour : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "CreditLastFourNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "CreditLastFourNumber", c => c.String());
        }
    }
}
