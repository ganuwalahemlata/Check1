namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTableForAutoTransactionQueueEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionQueueAutoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoOfTransactions = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProcessorId = c.Int(nullable: false),
                        CardType = c.String(),
                        RemainingTransactions = c.Int(nullable: false),
                        Date = c.DateTime(),
                        finished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processors", t => t.ProcessorId, cascadeDelete: true)
                .Index(t => t.ProcessorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionQueueAutoes", "ProcessorId", "dbo.Processors");
            DropIndex("dbo.TransactionQueueAutoes", new[] { "ProcessorId" });
            DropTable("dbo.TransactionQueueAutoes");
        }
    }
}
