namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class order_created : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "Created", c => c.DateTime());
        }
    }
}
