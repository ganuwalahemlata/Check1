﻿namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Update_NoOfDaysColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "NoOfDays", c => c.String());
        }

        public override void Down()
        {
            AddColumn("dbo.Events", "NoOfDays", c => c.String());
        }
    }
}
