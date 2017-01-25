namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order_prepaid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Prepaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Prepaid");
        }
    }
}
