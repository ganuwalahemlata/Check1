namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partial_lastupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partials", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.Partials", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partials", "Created");
            DropColumn("dbo.Partials", "LastUpdate");
        }
    }
}
