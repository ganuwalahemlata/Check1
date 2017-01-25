namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewMigrationName : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionQueueMasters",
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
                .ForeignKey("dbo.Processors", t => t.ProcessorId, cascadeDelete: true)
                .Index(t => t.PrepaidCardId)
                .Index(t => t.ProcessorId);
            
            CreateTable(
                "dbo.TriggerEmailTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        sent = c.Boolean(nullable: false),
                        Type = c.Int(nullable: false),
                        TriggerTime = c.DateTime(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Events", "NoOfDays", c => c.String());
            AddColumn("dbo.Events", "SecondTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "BIN", c => c.String());
            AddColumn("dbo.Orders", "LastFour", c => c.String());
            AddColumn("dbo.OrderProducts", "NextSalvageProfileId", c => c.Int());
            AlterColumn("dbo.Products", "Weight", c => c.Int());
            CreateIndex("dbo.OrderProducts", "NextSalvageProfileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionQueues", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.TransactionQueueMasters", "ProcessorId", "dbo.Processors");
            DropIndex("dbo.TransactionQueues", new[] { "ProcessorId" });
            DropIndex("dbo.TransactionQueues", new[] { "PrepaidCardId" });
            DropIndex("dbo.TransactionQueueMasters", new[] { "ProcessorId" });
            DropIndex("dbo.OrderProducts", new[] { "NextSalvageProfileId" });
            AlterColumn("dbo.Products", "Weight", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.OrderProducts", "NextSalvageProfileId");
            DropColumn("dbo.Orders", "LastFour");
            DropColumn("dbo.Orders", "BIN");
            DropColumn("dbo.Events", "SecondTemplateId");
            DropColumn("dbo.Events", "NoOfDays");
            DropTable("dbo.TriggerEmailTables");
            DropTable("dbo.TransactionQueues");
            DropTable("dbo.TransactionQueueMasters");
        }
    }
}
