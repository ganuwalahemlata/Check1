namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class processor_remove_usedeclinesalvage : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Processors", "UseDeclineSalvage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Processors", "UseDeclineSalvage", c => c.Boolean(nullable: false));
        }
    }
}
