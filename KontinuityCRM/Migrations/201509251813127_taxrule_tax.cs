namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taxrule_tax : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TaxRules", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaxRules", "Tax", c => c.Int(nullable: false));
        }
    }
}
