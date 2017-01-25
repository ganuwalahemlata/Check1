namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order_rebill_depth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Depth", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Depth");
        }
    }
}
