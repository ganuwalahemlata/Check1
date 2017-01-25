namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taxprofile_required_name : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TaxProfiles", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaxProfiles", "Name", c => c.String());
        }
    }
}
