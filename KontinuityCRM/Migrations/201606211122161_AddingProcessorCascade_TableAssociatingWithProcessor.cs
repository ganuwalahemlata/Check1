namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingProcessorCascade_TableAssociatingWithProcessor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProcessorCascades",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ProcessorId = c.Int(),
                    ProcessorRetryId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processors", t => t.ProcessorId)
                .ForeignKey("dbo.Processors", t => t.ProcessorRetryId)
                .Index(t => t.ProcessorId)
                .Index(t => t.ProcessorRetryId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ProcessorCascades", "ProcessorRetryId", "dbo.Processors");
            DropForeignKey("dbo.ProcessorCascades", "ProcessorId", "dbo.Processors");
            DropIndex("dbo.ProcessorCascades", new[] { "ProcessorRetryId" });
            DropIndex("dbo.ProcessorCascades", new[] { "ProcessorId" });
            DropTable("dbo.ProcessorCascades");
        }
    }
}
