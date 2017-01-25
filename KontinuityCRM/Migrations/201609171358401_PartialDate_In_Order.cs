namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartialDate_In_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PartialDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PartialDate");
        }
    }
}
