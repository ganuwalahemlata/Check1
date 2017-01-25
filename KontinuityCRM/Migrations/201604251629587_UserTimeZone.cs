using System.Data.Entity.Migrations;

namespace KontinuityCRM.Migrations
{
    public partial class UserTimeZone : DbMigration
    {
        private void PopulateTimeZonesTbl()
        {
            string sql = "";
            sql += @"
                SET IDENTITY_INSERT [TimeZones] ON                
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('1', 'Morocco Standard Time', '(UTC) Casablanca')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('2', 'UTC', '(UTC) Coordinated Universal Time')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('3', 'GMT Standard Time', '(UTC) Dublin, Edinburgh, Lisbon, London')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('4', 'Greenwich Standard Time', '(UTC) Monrovia, Reykjavik')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('5', 'W. Europe Standard Time', '(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('6', 'Central Europe Standard Time', '(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('7', 'Romance Standard Time', '(UTC+01:00) Brussels, Copenhagen, Madrid, Paris')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('8', 'Central European Standard Time', '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('9', 'W. Central Africa Standard Time', '(UTC+01:00) West Central Africa')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('10', 'Namibia Standard Time', '(UTC+01:00) Windhoek')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('11', 'Jordan Standard Time', '(UTC+02:00) Amman')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('12', 'GTB Standard Time', '(UTC+02:00) Athens, Bucharest')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('13', 'Middle East Standard Time', '(UTC+02:00) Beirut')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('14', 'Egypt Standard Time', '(UTC+02:00) Cairo')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('15', 'Syria Standard Time', '(UTC+02:00) Damascus')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('16', 'E. Europe Standard Time', '(UTC+02:00) E. Europe')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('17', 'South Africa Standard Time', '(UTC+02:00) Harare, Pretoria')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('18', 'FLE Standard Time', '(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('19', 'Turkey Standard Time', '(UTC+02:00) Istanbul')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('20', 'Israel Standard Time', '(UTC+02:00) Jerusalem')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('21', 'Kaliningrad Standard Time', '(UTC+02:00) Kaliningrad')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('22', 'Libya Standard Time', '(UTC+02:00) Tripoli')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('23', 'Arabic Standard Time', '(UTC+03:00) Baghdad')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('24', 'Arab Standard Time', '(UTC+03:00) Kuwait, Riyadh')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('25', 'Belarus Standard Time', '(UTC+03:00) Minsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('26', 'Russian Standard Time', '(UTC+03:00) Moscow, St. Petersburg, Volgograd')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('27', 'E. Africa Standard Time', '(UTC+03:00) Nairobi')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('28', 'Iran Standard Time', '(UTC+03:30) Tehran')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('29', 'Arabian Standard Time', '(UTC+04:00) Abu Dhabi, Muscat')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('30', 'Astrakhan Standard Time', '(UTC+04:00) Astrakhan, Ulyanovsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('31', 'Azerbaijan Standard Time', '(UTC+04:00) Baku')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('32', 'Russia Time Zone 3', '(UTC+04:00) Izhevsk, Samara')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('33', 'Mauritius Standard Time', '(UTC+04:00) Port Louis')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('34', 'Georgian Standard Time', '(UTC+04:00) Tbilisi')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('35', 'Caucasus Standard Time', '(UTC+04:00) Yerevan')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('36', 'Afghanistan Standard Time', '(UTC+04:30) Kabul')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('37', 'West Asia Standard Time', '(UTC+05:00) Ashgabat, Tashkent')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('38', 'Ekaterinburg Standard Time', '(UTC+05:00) Ekaterinburg')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('39', 'Pakistan Standard Time', '(UTC+05:00) Islamabad, Karachi')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('40', 'India Standard Time', '(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('41', 'Sri Lanka Standard Time', '(UTC+05:30) Sri Jayawardenepura')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('42', 'Nepal Standard Time', '(UTC+05:45) Kathmandu')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('43', 'Central Asia Standard Time', '(UTC+06:00) Astana')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('44', 'Bangladesh Standard Time', '(UTC+06:00) Dhaka')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('45', 'N. Central Asia Standard Time', '(UTC+06:00) Novosibirsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('46', 'Myanmar Standard Time', '(UTC+06:30) Yangon (Rangoon)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('47', 'SE Asia Standard Time', '(UTC+07:00) Bangkok, Hanoi, Jakarta')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('48', 'Altai Standard Time', '(UTC+07:00) Barnaul, Gorno-Altaysk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('49', 'North Asia Standard Time', '(UTC+07:00) Krasnoyarsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('50', 'China Standard Time', '(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('51', 'North Asia East Standard Time', '(UTC+08:00) Irkutsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('52', 'Singapore Standard Time', '(UTC+08:00) Kuala Lumpur, Singapore')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('53', 'W. Australia Standard Time', '(UTC+08:00) Perth')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('54', 'Taipei Standard Time', '(UTC+08:00) Taipei')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('55', 'Ulaanbaatar Standard Time', '(UTC+08:00) Ulaanbaatar')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('56', 'North Korea Standard Time', '(UTC+08:30) Pyongyang')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('57', 'Transbaikal Standard Time', '(UTC+09:00) Chita')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('58', 'Tokyo Standard Time', '(UTC+09:00) Osaka, Sapporo, Tokyo')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('59', 'Korea Standard Time', '(UTC+09:00) Seoul')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('60', 'Yakutsk Standard Time', '(UTC+09:00) Yakutsk')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('61', 'Cen. Australia Standard Time', '(UTC+09:30) Adelaide')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('62', 'AUS Central Standard Time', '(UTC+09:30) Darwin')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('63', 'E. Australia Standard Time', '(UTC+10:00) Brisbane')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('64', 'AUS Eastern Standard Time', '(UTC+10:00) Canberra, Melbourne, Sydney')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('65', 'West Pacific Standard Time', '(UTC+10:00) Guam, Port Moresby')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('66', 'Tasmania Standard Time', '(UTC+10:00) Hobart')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('67', 'Magadan Standard Time', '(UTC+10:00) Magadan')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('68', 'Vladivostok Standard Time', '(UTC+10:00) Vladivostok')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('69', 'Russia Time Zone 10', '(UTC+11:00) Chokurdakh')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('70', 'Sakhalin Standard Time', '(UTC+11:00) Sakhalin')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('71', 'Central Pacific Standard Time', '(UTC+11:00) Solomon Is., New Caledonia')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('72', 'Russia Time Zone 11', '(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('73', 'New Zealand Standard Time', '(UTC+12:00) Auckland, Wellington')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('74', 'UTC+12', '(UTC+12:00) Coordinated Universal Time+12')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('75', 'Fiji Standard Time', '(UTC+12:00) Fiji')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('76', 'Kamchatka Standard Time', '(UTC+12:00) Petropavlovsk-Kamchatsky - Old')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('77', 'Tonga Standard Time', '(UTC+13:00) Nuku''alofa')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('78', 'Samoa Standard Time', '(UTC+13:00) Samoa')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('79', 'Line Islands Standard Time', '(UTC+14:00) Kiritimati Island')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('80', 'Azores Standard Time', '(UTC-01:00) Azores')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('81', 'Cape Verde Standard Time', '(UTC-01:00) Cabo Verde Is.')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('82', 'UTC-02', '(UTC-02:00) Coordinated Universal Time-02')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('83', 'Mid-Atlantic Standard Time', '(UTC-02:00) Mid-Atlantic - Old')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('84', 'E. South America Standard Time', '(UTC-03:00) Brasilia')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('85', 'SA Eastern Standard Time', '(UTC-03:00) Cayenne, Fortaleza')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('86', 'Argentina Standard Time', '(UTC-03:00) City of Buenos Aires')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('87', 'Greenland Standard Time', '(UTC-03:00) Greenland')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('88', 'Montevideo Standard Time', '(UTC-03:00) Montevideo')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('89', 'Bahia Standard Time', '(UTC-03:00) Salvador')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('90', 'Pacific SA Standard Time', '(UTC-03:00) Santiago')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('91', 'Newfoundland Standard Time', '(UTC-03:30) Newfoundland')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('92', 'Paraguay Standard Time', '(UTC-04:00) Asuncion')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('93', 'Atlantic Standard Time', '(UTC-04:00) Atlantic Time (Canada)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('94', 'Central Brazilian Standard Time', '(UTC-04:00) Cuiaba')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('95', 'SA Western Standard Time', '(UTC-04:00) Georgetown, La Paz, Manaus, San Juan')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('96', 'Venezuela Standard Time', '(UTC-04:30) Caracas')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('97', 'SA Pacific Standard Time', '(UTC-05:00) Bogota, Lima, Quito, Rio Branco')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('98', 'Eastern Standard Time (Mexico)', '(UTC-05:00) Chetumal')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('99', 'Eastern Standard Time', '(UTC-05:00) Eastern Time (US & Canada)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('100', 'US Eastern Standard Time', '(UTC-05:00) Indiana (East)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('101', 'Central America Standard Time', '(UTC-06:00) Central America')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('102', 'Central Standard Time', '(UTC-06:00) Central Time (US & Canada)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('103', 'Central Standard Time (Mexico)', '(UTC-06:00) Guadalajara, Mexico City, Monterrey')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('104', 'Canada Central Standard Time', '(UTC-06:00) Saskatchewan')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('105', 'US Mountain Standard Time', '(UTC-07:00) Arizona')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('106', 'Mountain Standard Time (Mexico)', '(UTC-07:00) Chihuahua, La Paz, Mazatlan')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('107', 'Mountain Standard Time', '(UTC-07:00) Mountain Time (US & Canada)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('108', 'Pacific Standard Time (Mexico)', '(UTC-08:00) Baja California')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('109', 'Pacific Standard Time', '(UTC-08:00) Pacific Time (US & Canada)')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('110', 'Alaskan Standard Time', '(UTC-09:00) Alaska')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('111', 'Hawaiian Standard Time', '(UTC-10:00) Hawaii')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('112', 'UTC-11', '(UTC-11:00) Coordinated Universal Time-11')
                INSERT INTO TimeZones([Id], [Name], [DisplayName]) VALUES ('113', 'Dateline Standard Time', '(UTC-12:00) International Date Line West')
                SET IDENTITY_INSERT [TimeZones] OFF
                ";
            Sql(sql);
        }

        public override void Up()
        {
            CreateTable(
                "TimeZones",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false),
                    DisplayName = c.String(false),
                })
                .PrimaryKey(t => t.Id);
            PopulateTimeZonesTbl();

            AddColumn("UserProfile", "TimeZoneId", c => c.Int(true));
            Sql("UPDATE UserProfile SET TimeZoneId = 2"); //user timezone set to UTC/(UTC) Coordinated Universal Time
            AlterColumn("UserProfile", "TimeZoneId", c => c.Int(false));
            AddForeignKey(principalTable: "TimeZones", principalColumn: "Id", dependentTable: "UserProfile",
                dependentColumn: "TimeZoneId", cascadeDelete: false);
        }

        public override void Down()
        {
            DropForeignKey(principalTable: "TimeZones", dependentTable: "UserProfile", dependentColumn: "TimeZoneId");
            DropColumn("UserProfile", "TimeZoneId");
            DropTable("TimeZones");
        }
    }
}
