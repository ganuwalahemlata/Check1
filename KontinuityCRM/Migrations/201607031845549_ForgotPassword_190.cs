namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForgotPassword_190 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "ForgotPasswordToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "ForgotPasswordToken");
        }
    }
}
