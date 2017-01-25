namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMANumberCreatedOn_ExpirationDays_AddedIn_Order_RMAReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RMANumberCreatedOn", c => c.DateTime());
            AddColumn("dbo.RMAReasons", "ExpirationDays", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMAReasons", "ExpirationDays");
            DropColumn("dbo.Orders", "RMANumberCreatedOn");
        }
    }
}
