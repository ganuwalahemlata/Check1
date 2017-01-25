namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class isOrderCreatedFromPartial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "isFromPartial", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Orders", "isFromPartial");
        }
    }
}
