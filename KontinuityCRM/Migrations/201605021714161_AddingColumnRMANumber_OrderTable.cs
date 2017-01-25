namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingColumnRMANumber_OrderTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RMANumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "RMANumber");
        }
    }
}
