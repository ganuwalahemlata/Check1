namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTriggerEmailTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TriggerEmailTable",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    OrderId = c.Int(nullable: true),
                    EventId = c.Int(nullable: true),

                    UserName = c.String(),
                    TriggerTime = c.DateTime(nullable: false),
                    Type = c.Int(nullable: false),
                   
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.TriggerEmailTable");
        }
    }
}
