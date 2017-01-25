namespace KontinuityCRM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first_20150623 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        EventDateUTC = c.DateTimeOffset(nullable: false, precision: 7),
                        EventType = c.Int(nullable: false),
                        TableName = c.String(nullable: false, maxLength: 256),
                        RecordId = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.AuditLogId);
            
            CreateTable(
                "dbo.AuditLogDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColumnName = c.String(nullable: false, maxLength: 256),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                        AuditLogId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "dbo.AutoResponderProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Alias = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UserName = c.String(),
                        ApiKey = c.String(),
                        ApiSecret = c.String(),
                        ApiPassword = c.String(),
                        ApiEndPoint = c.String(),
                        Password = c.String(),
                        CustomField1Name = c.String(),
                        CustomField1Value = c.String(),
                        CustomField2Name = c.String(),
                        CustomField2Value = c.String(),
                        CustomField3Name = c.String(),
                        CustomField3Value = c.String(),
                        CustomField4Name = c.String(),
                        CustomField4Value = c.String(),
                        CustomField5Name = c.String(),
                        CustomField5Value = c.String(),
                        Type = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BalancerProcessors",
                c => new
                    {
                        BalancerId = c.Int(nullable: false),
                        ProcessorId = c.Int(nullable: false),
                        AllocationPercent = c.Int(),
                        InitialLimit = c.Int(),
                        IsPreserved = c.Boolean(nullable: false),
                        Allocation = c.Decimal(precision: 18, scale: 2),
                        Initials = c.Int(nullable: false),
                        ProcessedAllocation = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.BalancerId, t.ProcessorId })
                .ForeignKey("dbo.Balancers", t => t.BalancerId, cascadeDelete: true)
                .ForeignKey("dbo.Processors", t => t.ProcessorId, cascadeDelete: true)
                .Index(t => t.BalancerId)
                .Index(t => t.ProcessorId);
            
            CreateTable(
                "dbo.Balancers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AllocationBalance = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Processors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GatewayId = c.Int(nullable: false),
                        Parameters = c.Binary(),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Descriptor = c.String(),
                        CustomerServiceNumber = c.String(),
                        GlobalMonthlyCap = c.String(),
                        TransactionFee = c.String(),
                        ChargebackFee = c.String(),
                        ProcessingPercent = c.String(),
                        ReversePercent = c.String(),
                        CaptureOnShipment = c.Boolean(nullable: false),
                        ShipmentOnCapture = c.Boolean(nullable: false),
                        CaptureDelayHours = c.Int(),
                        Currency = c.String(),
                        PostProcessorId = c.Boolean(nullable: false),
                        PostProductDescription = c.Boolean(nullable: false),
                        PostDescriptor = c.Boolean(nullable: false),
                        UsePreAuthorizationFilter = c.Boolean(nullable: false),
                        UseDeclineSalvage = c.Boolean(nullable: false),
                        SiteId = c.String(),
                        MerchantAccountId = c.String(),
                        DynamicProductId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gateways", t => t.GatewayId, cascadeDelete: true)
                .Index(t => t.GatewayId);
            
            CreateTable(
                "dbo.Gateways",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Type = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        SKU = c.String(),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.Int(nullable: false),
                        IsTaxable = c.Boolean(nullable: false),
                        IsShippable = c.Boolean(nullable: false),
                        Weight = c.Int(),
                        ShipValue = c.Int(),
                        IsSignatureConfirmation = c.Boolean(nullable: false),
                        FulfillmentProviderId = c.Int(),
                        IsDeliveryConfirmation = c.Boolean(nullable: false),
                        IsSubscription = c.Boolean(nullable: false),
                        RecurringProductId = c.Int(),
                        BillType = c.Int(nullable: false),
                        BillValue = c.String(),
                        LoadBalancer = c.Boolean(nullable: false),
                        BalancerId = c.Int(),
                        ProcessorId = c.Int(),
                        HasRedemption = c.Boolean(nullable: false),
                        SalePoints = c.Int(),
                        RedemptionPoints = c.Int(),
                        AutoResponderProviderId = c.Int(),
                        AutoResponderProspectId = c.String(),
                        AutoResponderCustomerId = c.String(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.AutoResponderProviders", t => t.AutoResponderProviderId)
                .ForeignKey("dbo.Balancers", t => t.BalancerId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.FulfillmentProviders", t => t.FulfillmentProviderId)
                .ForeignKey("dbo.Processors", t => t.ProcessorId)
                .ForeignKey("dbo.Products", t => t.RecurringProductId)
                .Index(t => t.FulfillmentProviderId)
                .Index(t => t.RecurringProductId)
                .Index(t => t.BalancerId)
                .Index(t => t.ProcessorId)
                .Index(t => t.AutoResponderProviderId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.DeclineSalvages",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        BillType = c.Int(nullable: false),
                        BillValue = c.String(),
                        CancelAfter = c.Int(nullable: false),
                        LowerPrice = c.Boolean(nullable: false),
                        LowerPriceAfter = c.Int(),
                        LowerPercent = c.Int(),
                        LowerAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductEvents",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductId, t.EventId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.EventId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Publish = c.Boolean(nullable: false),
                        Type = c.Int(nullable: false),
                        TemplateId = c.Int(nullable: false),
                        SmtpServerId = c.Int(nullable: false),
                        Description = c.String(),
                        CreatedUserId = c.Int(),
                        UpdatedUserId = c.Int(),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.CreatedUserId)
                .ForeignKey("dbo.SmtpServers", t => t.SmtpServerId, cascadeDelete: true)
                .ForeignKey("dbo.EmailTemplates", t => t.TemplateId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedUserId)
                .Index(t => t.TemplateId)
                .Index(t => t.SmtpServerId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.UpdatedUserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Permissions = c.Long(),
                        Permissions1 = c.Long(),
                        Permissions2 = c.Long(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.SmtpServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Host = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        From = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        Authorization = c.Boolean(nullable: false),
                        Publish = c.Boolean(nullable: false),
                        Authenticated = c.Boolean(nullable: false),
                        LastUpdate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.CreatedUserId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedUserId)
                .Index(t => t.CreatedUserId)
                .Index(t => t.UpdatedUserId);
            
            CreateTable(
                "dbo.EmailTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        Subject = c.String(nullable: false),
                        Publish = c.Boolean(nullable: false),
                        LastUpdate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        HtmlBody = c.String(),
                        TextBody = c.String(),
                        CreatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.CreatedUserId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedUserId)
                .Index(t => t.UpdatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.FulfillmentProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Delay = c.Int(),
                        Alias = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        RecieveTrackingId = c.Boolean(),
                        DataBase = c.String(),
                        ClientName = c.String(),
                        ApiKey = c.String(),
                        SixWorkSubDomains = c.String(),
                        Prefix = c.String(),
                        ReceiveReturns = c.Boolean(),
                        Type = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Partials",
                c => new
                    {
                        PartialId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        IPAddress = c.String(),
                        AffiliateId = c.Int(),
                        SubId = c.Int(),
                        ProductId = c.Int(),
                        ProviderResponse = c.String(),
                    })
                .PrimaryKey(t => t.PartialId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PostBackUrls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        ProductId = c.Int(nullable: false),
                        OrderType = c.Int(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        Payments = c.Int(nullable: false),
                        OrderActions = c.Int(nullable: false),
                        IsAction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        CountryAbbreviation = c.String(maxLength: 2, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        Province = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        IPAddress = c.String(),
                        ProviderResponse = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.DeclineReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        WildCard = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FormGenerations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        GenerationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        IPAddress = c.String(),
                        Url = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        NoteDate = c.DateTime(nullable: false),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        ShippingFirstName = c.String(nullable: false),
                        ShippingLastName = c.String(nullable: false),
                        ShippingAddress1 = c.String(nullable: false),
                        ShippingAddress2 = c.String(),
                        ShippingCity = c.String(nullable: false),
                        ShippingProvince = c.String(),
                        ShippingPostalCode = c.String(nullable: false),
                        ShippingCountry = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        BillingFirstName = c.String(nullable: false),
                        BillingLastName = c.String(nullable: false),
                        BillingAddress1 = c.String(nullable: false),
                        BillingAddress2 = c.String(),
                        BillingCity = c.String(nullable: false),
                        BillingProvince = c.String(),
                        BillingPostalCode = c.String(nullable: false),
                        BillingCountry = c.String(nullable: false),
                        ShippingMethodId = c.Int(nullable: false),
                        ProcessorId = c.Int(),
                        Shipped = c.Boolean(),
                        IPAddress = c.String(),
                        AffiliateId = c.Int(),
                        SubId = c.Int(),
                        ChargebackDate = c.DateTime(),
                        PaymentType = c.Int(nullable: false),
                        CreditCardNumber = c.String(nullable: false),
                        CreditCardExpirationMonth = c.Int(nullable: false),
                        CreditCardExpirationYear = c.Int(nullable: false),
                        CreditCardCVV = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedUserId = c.Int(),
                        LastUpdate = c.DateTime(),
                        Created = c.DateTime(),
                        ParentId = c.Int(),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalancerId = c.Int(),
                        CaptureDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Balancers", t => t.BalancerId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedUserId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.ParentId)
                .ForeignKey("dbo.Processors", t => t.ProcessorId)
                .ForeignKey("dbo.ShippingMethods", t => t.ShippingMethodId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.ShippingMethodId)
                .Index(t => t.ProcessorId)
                .Index(t => t.Status)
                .Index(t => t.CreatedUserId)
                .Index(t => t.ParentId)
                .Index(t => t.BalancerId);
            
            CreateTable(
                "dbo.OrderProducts",
                c => new
                    {
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        NextDate = c.DateTime(),
                        NextProductId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        RebillDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(precision: 18, scale: 2),
                        Shipped = c.Boolean(),
                        Recurring = c.Boolean(nullable: false),
                        ReAttempts = c.Int(nullable: false),
                        ChildOrderId = c.Int(),
                        RMAReasonId = c.Int(),
                        FulfillmentProviderResponse = c.String(),
                        FulfillmentDate = c.DateTime(),
                        Order_OrderId = c.Int(),
                    })
                .PrimaryKey(t => new { t.OrderId, t.ProductId })
                .ForeignKey("dbo.Orders", t => t.ChildOrderId)
                .ForeignKey("dbo.Products", t => t.NextProductId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.RMAReasons", t => t.RMAReasonId)
                .ForeignKey("dbo.Orders", t => t.Order_OrderId)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId)
                .Index(t => t.NextProductId)
                .Index(t => t.ChildOrderId)
                .Index(t => t.RMAReasonId)
                .Index(t => t.FulfillmentDate)
                .Index(t => t.Order_OrderId);
            
            CreateTable(
                "dbo.RMAReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Active = c.Boolean(nullable: false),
                        CustomAction = c.Boolean(nullable: false),
                        Action = c.Int(nullable: false),
                        ExpiredAction = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShippingMethods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RecurringPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShippingCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShippingCategories", t => t.ShippingCategoryId)
                .Index(t => t.ShippingCategoryId);
            
            CreateTable(
                "dbo.ShippingCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(),
                        LastUpdate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        CreatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.CreatedUserId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedUserId)
                .Index(t => t.UpdatedUserId)
                .Index(t => t.CreatedUserId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Response = c.String(),
                        Message = c.String(),
                        Type = c.Int(nullable: false),
                        TransactionId = c.String(),
                        Status = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        ProcessorId = c.Int(),
                        BalancerId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Success = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Balancers", t => t.BalancerId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Processors", t => t.ProcessorId)
                .Index(t => t.OrderId)
                .Index(t => t.ProcessorId)
                .Index(t => t.BalancerId);
            
            CreateTable(
                "dbo.Postbacks",
                c => new
                    {
                        PostbackId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PostbackURL = c.String(),
                        EventType = c.Int(nullable: false),
                        Parameters = c.Binary(),
                    })
                .PrimaryKey(t => t.PostbackId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.PrepaidInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BIN = c.String(nullable: false, maxLength: 6),
                        Prepaid = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.BIN, unique: true);
            
            CreateTable(
                "dbo.ProductVariants",
                c => new
                    {
                        ProductVariantId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        SKU = c.String(),
                        Cost = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Currency = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductVariantId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.ReturnProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VariantExtraFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductVariantId = c.Int(nullable: false),
                        FieldName = c.String(),
                        FieldValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductVariants", t => t.ProductVariantId, cascadeDelete: true)
                .Index(t => t.ProductVariantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantExtraFields", "ProductVariantId", "dbo.ProductVariants");
            DropForeignKey("dbo.ProductVariants", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductVariants", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Postbacks", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Transactions", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.Transactions", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Transactions", "BalancerId", "dbo.Balancers");
            DropForeignKey("dbo.Orders", "ShippingMethodId", "dbo.ShippingMethods");
            DropForeignKey("dbo.ShippingCategories", "UpdatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.ShippingMethods", "ShippingCategoryId", "dbo.ShippingCategories");
            DropForeignKey("dbo.ShippingCategories", "CreatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Orders", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.Orders", "ParentId", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "Order_OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "RMAReasonId", "dbo.RMAReasons");
            DropForeignKey("dbo.OrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderProducts", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "NextProductId", "dbo.Products");
            DropForeignKey("dbo.OrderProducts", "ChildOrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderNotes", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "CreatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Orders", "BalancerId", "dbo.Balancers");
            DropForeignKey("dbo.Products", "RecurringProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.PostBackUrls", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Partials", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "FulfillmentProviderId", "dbo.FulfillmentProviders");
            DropForeignKey("dbo.ProductEvents", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Events", "UpdatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Events", "TemplateId", "dbo.EmailTemplates");
            DropForeignKey("dbo.EmailTemplates", "UpdatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.EmailTemplates", "CreatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.Events", "SmtpServerId", "dbo.SmtpServers");
            DropForeignKey("dbo.SmtpServers", "UpdatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.SmtpServers", "CreatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductEvents", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "CreatedUserId", "dbo.UserProfile");
            DropForeignKey("dbo.DeclineSalvages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Products", "BalancerId", "dbo.Balancers");
            DropForeignKey("dbo.Products", "AutoResponderProviderId", "dbo.AutoResponderProviders");
            DropForeignKey("dbo.Processors", "GatewayId", "dbo.Gateways");
            DropForeignKey("dbo.BalancerProcessors", "ProcessorId", "dbo.Processors");
            DropForeignKey("dbo.BalancerProcessors", "BalancerId", "dbo.Balancers");
            DropForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
            DropIndex("dbo.VariantExtraFields", new[] { "ProductVariantId" });
            DropIndex("dbo.ProductVariants", new[] { "CountryId" });
            DropIndex("dbo.ProductVariants", new[] { "ProductId" });
            DropIndex("dbo.PrepaidInfoes", new[] { "BIN" });
            DropIndex("dbo.Postbacks", new[] { "ProductId" });
            DropIndex("dbo.Transactions", new[] { "BalancerId" });
            DropIndex("dbo.Transactions", new[] { "ProcessorId" });
            DropIndex("dbo.Transactions", new[] { "OrderId" });
            DropIndex("dbo.ShippingCategories", new[] { "CreatedUserId" });
            DropIndex("dbo.ShippingCategories", new[] { "UpdatedUserId" });
            DropIndex("dbo.ShippingMethods", new[] { "ShippingCategoryId" });
            DropIndex("dbo.OrderProducts", new[] { "Order_OrderId" });
            DropIndex("dbo.OrderProducts", new[] { "FulfillmentDate" });
            DropIndex("dbo.OrderProducts", new[] { "RMAReasonId" });
            DropIndex("dbo.OrderProducts", new[] { "ChildOrderId" });
            DropIndex("dbo.OrderProducts", new[] { "NextProductId" });
            DropIndex("dbo.OrderProducts", new[] { "ProductId" });
            DropIndex("dbo.OrderProducts", new[] { "OrderId" });
            DropIndex("dbo.Orders", new[] { "BalancerId" });
            DropIndex("dbo.Orders", new[] { "ParentId" });
            DropIndex("dbo.Orders", new[] { "CreatedUserId" });
            DropIndex("dbo.Orders", new[] { "Status" });
            DropIndex("dbo.Orders", new[] { "ProcessorId" });
            DropIndex("dbo.Orders", new[] { "ShippingMethodId" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropIndex("dbo.OrderNotes", new[] { "OrderId" });
            DropIndex("dbo.PostBackUrls", new[] { "ProductId" });
            DropIndex("dbo.Partials", new[] { "ProductId" });
            DropIndex("dbo.EmailTemplates", new[] { "CreatedUserId" });
            DropIndex("dbo.EmailTemplates", new[] { "UpdatedUserId" });
            DropIndex("dbo.SmtpServers", new[] { "UpdatedUserId" });
            DropIndex("dbo.SmtpServers", new[] { "CreatedUserId" });
            DropIndex("dbo.Events", new[] { "UpdatedUserId" });
            DropIndex("dbo.Events", new[] { "CreatedUserId" });
            DropIndex("dbo.Events", new[] { "SmtpServerId" });
            DropIndex("dbo.Events", new[] { "TemplateId" });
            DropIndex("dbo.ProductEvents", new[] { "EventId" });
            DropIndex("dbo.ProductEvents", new[] { "ProductId" });
            DropIndex("dbo.DeclineSalvages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Products", new[] { "AutoResponderProviderId" });
            DropIndex("dbo.Products", new[] { "ProcessorId" });
            DropIndex("dbo.Products", new[] { "BalancerId" });
            DropIndex("dbo.Products", new[] { "RecurringProductId" });
            DropIndex("dbo.Products", new[] { "FulfillmentProviderId" });
            DropIndex("dbo.Processors", new[] { "GatewayId" });
            DropIndex("dbo.BalancerProcessors", new[] { "ProcessorId" });
            DropIndex("dbo.BalancerProcessors", new[] { "BalancerId" });
            DropIndex("dbo.AuditLogDetails", new[] { "AuditLogId" });
            DropTable("dbo.VariantExtraFields");
            DropTable("dbo.ReturnProfiles");
            DropTable("dbo.ProductVariants");
            DropTable("dbo.PrepaidInfoes");
            DropTable("dbo.Postbacks");
            DropTable("dbo.Transactions");
            DropTable("dbo.ShippingCategories");
            DropTable("dbo.ShippingMethods");
            DropTable("dbo.RMAReasons");
            DropTable("dbo.OrderProducts");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderNotes");
            DropTable("dbo.KLogs");
            DropTable("dbo.FormGenerations");
            DropTable("dbo.DeclineReasons");
            DropTable("dbo.Customers");
            DropTable("dbo.Countries");
            DropTable("dbo.PostBackUrls");
            DropTable("dbo.Partials");
            DropTable("dbo.FulfillmentProviders");
            DropTable("dbo.EmailTemplates");
            DropTable("dbo.SmtpServers");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Events");
            DropTable("dbo.ProductEvents");
            DropTable("dbo.DeclineSalvages");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
            DropTable("dbo.Gateways");
            DropTable("dbo.Processors");
            DropTable("dbo.Balancers");
            DropTable("dbo.BalancerProcessors");
            DropTable("dbo.AutoResponderProviders");
            DropTable("dbo.AuditLogDetails");
            DropTable("dbo.AuditLogs");
        }
    }
}
