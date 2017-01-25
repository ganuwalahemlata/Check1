namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transaction_request : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Request", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "Request");
        }
    }
}
