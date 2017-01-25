namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCheckedForReprocess : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess", c => c.Boolean(nullable: false));
        }
    }
}
