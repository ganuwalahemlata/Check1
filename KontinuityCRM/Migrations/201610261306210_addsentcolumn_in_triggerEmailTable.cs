namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsentcolumn_in_triggerEmailTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TriggerEmailTables", "sent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TriggerEmailTables", "sent");
        }
    }
}
