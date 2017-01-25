namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taxrule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TaxProfiles", "CountryId", "dbo.Countries");
            DropIndex("dbo.TaxProfiles", new[] { "CountryId" });
            CreateTable(
                "dbo.TaxRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tax = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        TaxProfileId = c.Int(nullable: false),
                        City = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(),
                        ApplyToShipping = c.Boolean(nullable: false),
                        ShippingAddress = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.TaxProfiles", t => t.TaxProfileId, cascadeDelete: true)
                .Index(t => t.CountryId)
                .Index(t => t.TaxProfileId);
            
            AddColumn("dbo.UserProfile", "TaxRuleDisplay", c => c.Int(nullable: false, defaultValue:10));
            DropColumn("dbo.TaxProfiles", "Tax");
            DropColumn("dbo.TaxProfiles", "CountryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaxProfiles", "CountryId", c => c.Int(nullable: false));
            AddColumn("dbo.TaxProfiles", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.TaxRules", "TaxProfileId", "dbo.TaxProfiles");
            DropForeignKey("dbo.TaxRules", "CountryId", "dbo.Countries");
            DropIndex("dbo.TaxRules", new[] { "TaxProfileId" });
            DropIndex("dbo.TaxRules", new[] { "CountryId" });
            DropColumn("dbo.UserProfile", "TaxRuleDisplay");
            DropTable("dbo.TaxRules");
            CreateIndex("dbo.TaxProfiles", "CountryId");
            AddForeignKey("dbo.TaxProfiles", "CountryId", "dbo.Countries", "Id", cascadeDelete: true);
        }
    }
}
