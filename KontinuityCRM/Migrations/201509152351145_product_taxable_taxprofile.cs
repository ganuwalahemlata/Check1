namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class product_taxable_taxprofile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "TaxProfileId", c => c.Int());
            CreateIndex("dbo.Products", "TaxProfileId");
            AddForeignKey("dbo.Products", "TaxProfileId", "dbo.TaxProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "TaxProfileId", "dbo.TaxProfiles");
            DropIndex("dbo.Products", new[] { "TaxProfileId" });
            DropColumn("dbo.Products", "TaxProfileId");
        }
    }
}
