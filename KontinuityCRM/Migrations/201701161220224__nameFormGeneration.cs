namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _nameFormGeneration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormGenerations", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormGenerations", "Name");
        }
    }
}
