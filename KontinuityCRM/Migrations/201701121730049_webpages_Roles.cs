namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class webpages_Roles : DbMigration
    {
        public override void Up()
        {
            string query = @"IF not EXISTS (SELECT * FROM webpages_Roles where RoleName = 'EXPORT_save')
                   BEGIN
                       insert into webpages_Roles values('EXPORT_save')
                    END";
            Sql(query);
        }

        public override void Down()
        {
        }
    }
}
