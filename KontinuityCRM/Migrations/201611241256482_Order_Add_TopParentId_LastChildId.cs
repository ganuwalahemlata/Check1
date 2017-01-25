namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Order_Add_TopParentId_LastChildId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TopParentId", c => c.Int());
            AddColumn("dbo.Orders", "LastChildId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "TopParentId");
            DropColumn("dbo.Orders", "LastChildId");
        }
    }
}
