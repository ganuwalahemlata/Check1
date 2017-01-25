namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSecondTemplateIdColumnInEvent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "SecondTemplateId", "dbo.EmailTemplates");
            DropIndex("dbo.Events", new[] { "SecondTemplateId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Events", "SecondTemplateId");
            //AddForeignKey("dbo.Events", "SecondTemplateId", "dbo.EmailTemplates", "Id", cascadeDelete: true);
        }
    }
}
