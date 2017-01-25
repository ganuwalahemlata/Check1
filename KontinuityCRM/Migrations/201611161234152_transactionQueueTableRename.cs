namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactionQueueTableRename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TransactionQueueAutoes", newName: "TransactionQueueMasters");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.TransactionQueueMasters", newName: "TransactionQueueAutoes");
        }
    }
}
