namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Locations_Blocks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        startIpNum = c.Long(nullable: false),
                        endIpNum = c.Long(nullable: false),
                        locId = c.Int(),
                        registered_country_geoname_id = c.Int(),
                        represented_country_geoname_id = c.Int(),
                        is_anonymous_proxy = c.Boolean(nullable: false),
                        is_satellite_provider = c.Boolean(nullable: false),
                        postal_code = c.String(),
                        latitude = c.String(),
                        longitude = c.String(),
                    })
                .PrimaryKey(t => t.startIpNum)
                .ForeignKey("dbo.Locations", t => t.locId)
                .ForeignKey("dbo.Locations", t => t.registered_country_geoname_id)
                .ForeignKey("dbo.Locations", t => t.represented_country_geoname_id)
                .Index(t => t.endIpNum, unique: true)
                .Index(t => t.locId)
                .Index(t => t.registered_country_geoname_id)
                .Index(t => t.represented_country_geoname_id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        locId = c.Int(nullable: false),
                        locale_code = c.String(maxLength: 2, unicode: false),
                        continent_code = c.String(maxLength: 2, unicode: false),
                        continent_name = c.String(),
                        country_iso_code = c.String(maxLength: 2, unicode: false),
                        country_name = c.String(),
                        subdivision_1_iso_code = c.String(maxLength: 3, unicode: false),
                        subdivision_1_name = c.String(),
                        subdivision_2_iso_code = c.String(maxLength: 3, unicode: false),
                        subdivision_2_name = c.String(),
                        city_name = c.String(),
                        metro_code = c.String(),
                        time_zone = c.String(),
                    })
                .PrimaryKey(t => t.locId)
                .Index(t => t.country_iso_code);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blocks", "represented_country_geoname_id", "dbo.Locations");
            DropForeignKey("dbo.Blocks", "registered_country_geoname_id", "dbo.Locations");
            DropForeignKey("dbo.Blocks", "locId", "dbo.Locations");
            DropIndex("dbo.Locations", new[] { "country_iso_code" });
            DropIndex("dbo.Blocks", new[] { "represented_country_geoname_id" });
            DropIndex("dbo.Blocks", new[] { "registered_country_geoname_id" });
            DropIndex("dbo.Blocks", new[] { "locId" });
            DropIndex("dbo.Blocks", new[] { "endIpNum" });
            DropTable("dbo.Locations");
            DropTable("dbo.Blocks");
        }
    }
}
