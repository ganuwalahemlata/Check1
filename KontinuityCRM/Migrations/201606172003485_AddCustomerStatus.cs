namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Status", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Customers", "Status");
        }
    }
}
