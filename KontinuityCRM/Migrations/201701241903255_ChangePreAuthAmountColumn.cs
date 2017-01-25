namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePreAuthAmountColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "PreAuthAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "PreAuthAmount", c => c.Int());
        }
    }
}
