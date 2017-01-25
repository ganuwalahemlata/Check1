namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TransactionViaPrepaidCardTableUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionViaPrepaidCardQueues", "TransactionQueueMasterId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionViaPrepaidCardQueues", "TransactionQueueMasterId");
        }
    }
}
