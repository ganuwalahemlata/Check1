namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blacklistorderid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlackLists", "OrderId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlackLists", "OrderId");
        }
    }
}
