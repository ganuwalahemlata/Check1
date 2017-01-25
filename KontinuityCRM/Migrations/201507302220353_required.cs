namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalvageProfiles", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.DeclineTypes", "WildCard", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DeclineTypes", "WildCard", c => c.String());
            AlterColumn("dbo.SalvageProfiles", "Name", c => c.String());
        }
    }
}
