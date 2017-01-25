namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessorStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Processors", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Processors", "Status");
        }
    }
}
