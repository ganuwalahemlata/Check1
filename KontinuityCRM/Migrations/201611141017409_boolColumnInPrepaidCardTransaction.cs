namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boolColumnInPrepaidCardTransaction : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
          //  AlterColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess", c => c.Boolean());
        }
    }
}
