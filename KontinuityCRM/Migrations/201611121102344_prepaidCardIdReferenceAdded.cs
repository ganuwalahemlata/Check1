namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prepaidCardIdReferenceAdded : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId");
            AddForeignKey("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId", "dbo.PrepaidCards", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionViaPrepaidCardQueues", "PrepaidCardId", "dbo.PrepaidCards");
            DropIndex("dbo.TransactionViaPrepaidCardQueues", new[] { "PrepaidCardId" });
        }
    }
}
