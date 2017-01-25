namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PreAuthAmountColumnProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "PreAuthAmount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "PreAuthAmount");
        }
    }
}
