namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTableAddfortriggerEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TriggerEmailTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        TriggerTime = c.DateTime(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TriggerEmailTables");
        }
    }
}
