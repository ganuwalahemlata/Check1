namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExportFieldsForCSV : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExportFields",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Value = c.Boolean(nullable: false),
                    Order = c.Int(nullable: false),
                    ExportTemplate_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExportTemplates", t => t.ExportTemplate_Id)
                .Index(t => t.ExportTemplate_Id);

            AlterColumn("dbo.ExportTemplates", "Name", c => c.String(nullable: false));
        }

        public override void Down()
        {
            DropForeignKey("dbo.ExportFields", "ExportTemplate_Id", "dbo.ExportTemplates");
            DropIndex("dbo.ExportFields", new[] { "ExportTemplate_Id" });
            AlterColumn("dbo.ExportTemplates", "Name", c => c.String());
            DropTable("dbo.ExportFields");
        }

    }
}
