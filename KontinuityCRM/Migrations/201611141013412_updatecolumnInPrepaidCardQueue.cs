namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecolumnInPrepaidCardQueue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess", c => c.Boolean());
            //DropColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReporocess");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReporocess", c => c.Boolean(nullable: false));
            //DropColumn("dbo.TransactionViaPrepaidCardQueues", "checked_forReprocess");
        }
    }
}
