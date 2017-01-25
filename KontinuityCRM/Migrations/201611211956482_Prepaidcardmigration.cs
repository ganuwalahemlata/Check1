namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prepaidcardmigration : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Orders", "BIN", c => c.String());
            //AddColumn("dbo.Orders", "LastFour", c => c.String());
            AlterColumn("dbo.Products", "Weight", c => c.Int());
            DropColumn("dbo.UserProfile", "PrepaidCardDisplay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "PrepaidCardDisplay", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "Weight", c => c.Decimal(precision: 18, scale: 2));
            //DropColumn("dbo.Orders", "LastFour");
            //DropColumn("dbo.Orders", "BIN");
        }
    }
}
