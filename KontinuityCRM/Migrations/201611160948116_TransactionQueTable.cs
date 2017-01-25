namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionQueTable : DbMigration
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PrepaidCards", t => t.PrepaidCardId, cascadeDelete: true)
                .ForeignKey("dbo.Processors", t => t.ProcessorId, cascadeDelete: true)
                .Index(t => t.PrepaidCardId)
                .Index(t => t.ProcessorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionQueues", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.TransactionQueues", "PrepaidCardId", "dbo.PrepaidCards");
            DropIndex("dbo.TransactionQueues", new[] { "ProcessorId" });
            DropIndex("dbo.TransactionQueues", new[] { "PrepaidCardId" });
            DropTable("dbo.TransactionQueues");
        }
    }
}
