namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrepaidCardDisplayColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "PrepaidCardDisplay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "PrepaidCardDisplay");
        }
    }
}
