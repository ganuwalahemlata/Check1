namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Widgets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Widgets",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Type = c.Int(nullable: false),
                    isReportingWidget = c.Boolean(nullable: false),
                    Row_Position = c.Int(nullable: false),
                    Col_Position = c.Int(nullable: false),
                    CreatedOn = c.DateTime(nullable: false),
                    CreatedBy = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.Widgets");
        }
    }
}
