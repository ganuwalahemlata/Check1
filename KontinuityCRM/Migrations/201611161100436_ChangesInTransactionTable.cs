namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesInTransactionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionQueues", "TransactionQueMasterId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionQueues", "TransactionQueMasterId");
        }
    }
}
