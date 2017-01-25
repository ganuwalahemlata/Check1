namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactionreference : DbMigration
    {
        public override void Up()
        {

            AddColumn("dbo.Transactions", "TransactionReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "TransactionReference");
        }
    }
}