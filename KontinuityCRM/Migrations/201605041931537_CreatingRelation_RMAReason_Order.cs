namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatingRelation_RMAReason_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RMAReasonId", c => c.Int());
            CreateIndex("dbo.Orders", "RMAReasonId");
            AddForeignKey("dbo.Orders", "RMAReasonId", "dbo.RMAReasons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "RMAReasonId", "dbo.RMAReasons");
            DropIndex("dbo.Orders", new[] { "RMAReasonId" });
            DropColumn("dbo.Orders", "RMAReasonId");
        }
    }
}
