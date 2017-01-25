namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class declinesalvage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DeclineSalvages", "ProductId", "dbo.Products");
            DropIndex("dbo.DeclineSalvages", new[] { "ProductId" });
            CreateTable(
                "dbo.ProductSalvages",
                c => new
                    {
                        SalvageProfileId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SalvageProfileId, t.ProductId })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.SalvageProfiles", t => t.SalvageProfileId, cascadeDelete: true)
                .Index(t => t.SalvageProfileId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SalvageProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DeclineTypeId = c.Int(nullable: false),
                        BillType = c.Int(nullable: false),
                        BillValue = c.String(),
                        CancelAfter = c.Int(nullable: false),
                        LowerPrice = c.Boolean(nullable: false),
                        LowerPriceAfter = c.Int(),
                        LowerPercent = c.Int(),
                        LowerAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DeclineTypes", t => t.DeclineTypeId, cascadeDelete: true)
                .Index(t => t.DeclineTypeId);
            
            CreateTable(
                "dbo.DeclineTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        WildCard = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserProfile", "DeclineTypeDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "SalvageProfileDisplay", c => c.Int(nullable: false, defaultValue: 10));
            DropTable("dbo.DeclineSalvages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DeclineSalvages",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        BillType = c.Int(nullable: false),
                        BillValue = c.String(),
                        CancelAfter = c.Int(nullable: false),
                        LowerPrice = c.Boolean(nullable: false),
                        LowerPriceAfter = c.Int(),
                        LowerPercent = c.Int(),
                        LowerAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductId);
            
            DropForeignKey("dbo.ProductSalvages", "SalvageProfileId", "dbo.SalvageProfiles");
            DropForeignKey("dbo.SalvageProfiles", "DeclineTypeId", "dbo.DeclineTypes");
            DropForeignKey("dbo.ProductSalvages", "ProductId", "dbo.Products");
            DropIndex("dbo.SalvageProfiles", new[] { "DeclineTypeId" });
            DropIndex("dbo.ProductSalvages", new[] { "ProductId" });
            DropIndex("dbo.ProductSalvages", new[] { "SalvageProfileId" });
            DropColumn("dbo.UserProfile", "SalvageProfileDisplay");
            DropColumn("dbo.UserProfile", "DeclineTypeDisplay");
            DropTable("dbo.DeclineTypes");
            DropTable("dbo.SalvageProfiles");
            DropTable("dbo.ProductSalvages");
            CreateIndex("dbo.DeclineSalvages", "ProductId");
            AddForeignKey("dbo.DeclineSalvages", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
