namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExportTemplate_ExportTemplateFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExportTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExportTemplateFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldName = c.String(),
                        FieldValue = c.String(),
                        ExportTemplate_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExportTemplates", t => t.ExportTemplate_Id)
                .Index(t => t.ExportTemplate_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExportTemplateFields", "ExportTemplate_Id", "dbo.ExportTemplates");
            DropIndex("dbo.ExportTemplateFields", new[] { "ExportTemplate_Id" });
            DropTable("dbo.ExportTemplateFields");
            DropTable("dbo.ExportTemplates");
        }
    }
}
