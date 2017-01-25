namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNewColumnTransactionQueue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionQueues", "TimeIntervalMin", c => c.Int(nullable: false));
            AddColumn("dbo.TransactionQueues", "TimeIntervalMax", c => c.Int(nullable: false));
            AddColumn("dbo.TransactionQueues", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.TransactionQueues", "Attempt", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionQueues", "Attempt");
            DropColumn("dbo.TransactionQueues", "LastUpdatedDate");
            DropColumn("dbo.TransactionQueues", "TimeIntervalMax");
            DropColumn("dbo.TransactionQueues", "TimeIntervalMin");
        }
    }
}
