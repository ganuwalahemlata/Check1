namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FindTopParentId : DbMigration
    {
        public override void Up()
        {
            string sql = "";
            sql += @"CREATE FUNCTION [dbo].[GetParentID] 
            (
	            @OrderId int
            )
              RETURNS int
               AS
               BEGIN
                   RETURN
                   (
                       SELECT CASE WHEN ParentId IS NULL THEN OrderId ELSE dbo.GetParentID(ParentId) END
                       FROM dbo.Orders
                       WHERE OrderId = @OrderId
                   )
	            END";
            Sql(sql);
        }
        
        public override void Down()
        {
            string sql = "";
            sql += @"Drop FUNCTION [dbo].[GetParentID]";
            Sql(sql);
        }
    }
}
