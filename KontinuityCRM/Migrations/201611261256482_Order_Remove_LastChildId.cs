namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order_Remove_LastChildId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "LastChildId");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "LastChildId");
        }
    }
}
