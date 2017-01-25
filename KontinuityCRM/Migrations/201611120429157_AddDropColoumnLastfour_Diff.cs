namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDropColoumnLastfour_Diff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CreditLastFourNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CreditLastFourNumber");
        }
    }
}
