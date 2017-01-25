namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTopParentId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "TopParentId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "TopParentId");
        }
    }
}
