namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partial_required_productid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Partials", "ProductId", "dbo.Products");
            DropIndex("dbo.Partials", new[] { "ProductId" });
            AlterColumn("dbo.Partials", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.Partials", "ProductId");
            AddForeignKey("dbo.Partials", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partials", "ProductId", "dbo.Products");
            DropIndex("dbo.Partials", new[] { "ProductId" });
            AlterColumn("dbo.Partials", "ProductId", c => c.Int());
            CreateIndex("dbo.Partials", "ProductId");
            AddForeignKey("dbo.Partials", "ProductId", "dbo.Products", "ProductId");
        }
    }
}
