namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PorcessorSpecificText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Processors", "ProcessorSpecificText", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Processors", "ProcessorSpecificText");
        }
    }
}
