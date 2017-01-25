namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Column_Added_PaymentTypeId_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PaymentTypeId", c => c.Int());
            CreateIndex("dbo.Orders", "PaymentTypeId");
            AddForeignKey("dbo.Orders", "PaymentTypeId", "dbo.PaymentTypes", "PaymentTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "PaymentTypeId", "dbo.PaymentTypes");
            DropIndex("dbo.Orders", new[] { "PaymentTypeId" });
            DropColumn("dbo.Orders", "PaymentTypeId");
        }
    }
}
