namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackingNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "TrackingNo", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "TrackingNo");
        }
    }
}
