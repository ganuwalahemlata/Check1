namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBINMig : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Orders", "CreditLastFourNumber", "LastFour");
        }
        
        public override void Down()
        {
        }
    }
}
