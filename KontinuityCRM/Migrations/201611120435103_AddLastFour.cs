namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastFour : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "LastFour", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "LastFour");
        }
    }
}
