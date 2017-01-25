namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taxprofiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaxProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            AddColumn("dbo.UserProfile", "TaxProfileDisplay", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaxProfiles", "CountryId", "dbo.Countries");
            DropIndex("dbo.TaxProfiles", new[] { "CountryId" });
            DropColumn("dbo.UserProfile", "TaxProfileDisplay");
            DropTable("dbo.TaxProfiles");
        }
    }
}
