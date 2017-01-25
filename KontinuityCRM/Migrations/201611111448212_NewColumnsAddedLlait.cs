namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewColumnsAddedLlait : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransactionViaPrepaidCardQueues", "BalancerId", "dbo.Balancers");
            DropForeignKey("dbo.TransactionViaPrepaidCardQueues", "ProcessorId", "dbo.Processors");
            DropIndex("dbo.TransactionViaPrepaidCardQueues", new[] { "ProcessorId" });
            DropIndex("dbo.TransactionViaPrepaidCardQueues", new[] { "BalancerId" });
            AddColumn("dbo.Orders", "BIN", c => c.String());
            AddColumn("dbo.Orders", "CreditLastFourNumber", c => c.String());
            DropTable("dbo.PrepaidCards");
            DropTable("dbo.PrepaidCardTransactionQueues");
            DropTable("dbo.TransactionViaPrepaidCardQueues");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TransactionViaPrepaidCardQueues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Request = c.String(),
                        Response = c.String(),
                        Message = c.String(),
                        Type = c.Int(nullable: false),
                        TransactionId = c.String(),
                        TransactionReference = c.String(),
                        Status = c.Int(nullable: false),
                        ProcessorId = c.Int(),
                        BalancerId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Success = c.Boolean(nullable: false),
                        PrepaidCardId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrepaidCardTransactionQueues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        no_of_transactions = c.String(nullable: false),
                        amount = c.String(nullable: false),
                        ProcessorID = c.String(nullable: false),
                        Date = c.DateTime(),
                        stop = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrepaidCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        First_Name = c.String(nullable: false),
                        Last_Name = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Zip = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Amount = c.String(nullable: false),
                        Number = c.String(nullable: false),
                        PaymentType = c.String(nullable: false),
                        CreditCardExpirationMonth = c.String(nullable: false),
                        CreditCardExpirationYear = c.String(nullable: false),
                        CreditCardCVV = c.String(nullable: false),
                        Date = c.DateTime(),
                        RemainingAmount = c.String(),
                        declined = c.Boolean(nullable: false),
                        Stop = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Orders", "CreditLastFourNumber");
            DropColumn("dbo.Orders", "BIN");
            CreateIndex("dbo.TransactionViaPrepaidCardQueues", "BalancerId");
            CreateIndex("dbo.TransactionViaPrepaidCardQueues", "ProcessorId");
            AddForeignKey("dbo.TransactionViaPrepaidCardQueues", "ProcessorId", "dbo.Processors", "Id");
            AddForeignKey("dbo.TransactionViaPrepaidCardQueues", "BalancerId", "dbo.Balancers", "Id");
        }
    }
}
