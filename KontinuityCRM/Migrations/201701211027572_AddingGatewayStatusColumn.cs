namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingGatewayStatusColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gateways", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Gateways", "Status");
        }
    }
}
