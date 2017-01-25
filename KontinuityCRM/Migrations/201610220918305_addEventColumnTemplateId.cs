namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEventColumnTemplateId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "SecondTemplateId", c => c.Int(nullable: false));
           CreateIndex("dbo.Events", "SecondTemplateId");
           AddForeignKey("dbo.Events", "SecondTemplateId", "dbo.EmailTemplates", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "SecondTemplateId", "dbo.EmailTemplates");
            DropIndex("dbo.Events", new[] { "SecondTemplateId" });
            DropColumn("dbo.Events", "SecondTemplateId");
        }
    }
}
