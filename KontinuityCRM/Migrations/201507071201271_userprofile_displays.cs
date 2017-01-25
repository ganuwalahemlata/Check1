namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userprofile_displays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "CategoryDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ProductDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "OrderDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ShippingMethodDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ShippingCategoryDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "BalancerDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "GatewayDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "FulfillmentDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "AutoresponderDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ProcessorDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "EmailTemplateDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "RMAReasonDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "UserDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "CustomerDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ObjectLogDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "ViewLogDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "PartialDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "AuditLogDisplay", c => c.Int(nullable: false, defaultValue: 10));
            AddColumn("dbo.UserProfile", "KLogDisplay", c => c.Int(nullable: false, defaultValue: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "KLogDisplay");
            DropColumn("dbo.UserProfile", "AuditLogDisplay");
            DropColumn("dbo.UserProfile", "PartialDisplay");
            DropColumn("dbo.UserProfile", "ViewLogDisplay");
            DropColumn("dbo.UserProfile", "ObjectLogDisplay");
            DropColumn("dbo.UserProfile", "CustomerDisplay");
            DropColumn("dbo.UserProfile", "UserDisplay");
            DropColumn("dbo.UserProfile", "RMAReasonDisplay");
            DropColumn("dbo.UserProfile", "EmailTemplateDisplay");
            DropColumn("dbo.UserProfile", "ProcessorDisplay");
            DropColumn("dbo.UserProfile", "AutoresponderDisplay");
            DropColumn("dbo.UserProfile", "FulfillmentDisplay");
            DropColumn("dbo.UserProfile", "GatewayDisplay");
            DropColumn("dbo.UserProfile", "BalancerDisplay");
            DropColumn("dbo.UserProfile", "ShippingCategoryDisplay");
            DropColumn("dbo.UserProfile", "ShippingMethodDisplay");
            DropColumn("dbo.UserProfile", "OrderDisplay");
            DropColumn("dbo.UserProfile", "ProductDisplay");
            DropColumn("dbo.UserProfile", "CategoryDisplay");
        }
    }
}
