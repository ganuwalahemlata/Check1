namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lalit_new_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.TransactionQueues",
               c => new
               {
                   Id = c.Int(nullable: false, identity: true),
                   PrepaidCardId = c.Int(nullable: false),
                   Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                   ProcessorId = c.Int(nullable: false),
                   Date = c.DateTime(nullable: false),
                   finished = c.Boolean(nullable: false),
                   TransactionQueMasterId = c.Int(nullable: false),
               })
               .PrimaryKey(t => t.Id)
               .ForeignKey("dbo.PrepaidCards", t => t.PrepaidCardId, cascadeDelete: true)
               .ForeignKey("dbo.Processors", t => t.ProcessorId, cascadeDelete: true)
               .Index(t => t.PrepaidCardId)
               .Index(t => t.ProcessorId);

            AddColumn("dbo.UserProfile", "PrepaidCardDisplay", c => c.Int(nullable: false));
            AddColumn("dbo.PrepaidCards", "Country", c => c.String(nullable: false));
            AddColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess", c => c.Boolean(nullable: false));
            CreateIndex("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId");
            AddForeignKey("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId", "dbo.PrepaidCards", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {

            DropForeignKey("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId", "dbo.PrepaidCards");
            DropForeignKey("dbo.TransactionQueues", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.TransactionQueues", "PrepaidCardId", "dbo.PrepaidCards");
            DropIndex("dbo.TransactionViaPrepaidCardQueues", new[] { "PrepaidCardId" });
            DropIndex("dbo.TransactionQueues", new[] { "ProcessorId" });
            DropIndex("dbo.TransactionQueues", new[] { "PrepaidCardId" });
            DropColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess");
            DropColumn("dbo.PrepaidCards", "Country");
            DropColumn("dbo.UserProfile", "PrepaidCardDisplay");
            DropTable("dbo.TransactionQueues");
        }
    }
}
