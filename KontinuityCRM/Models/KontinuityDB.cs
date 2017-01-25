//using KontinuityCRM.Helpers.GRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KontinuityCRM.Helpers;
using TrackerEnabledDbContext;

namespace KontinuityCRM.Models
{
    public class KontinuityDB : TrackerContext //DbContext,
        ,IContinuityDbContext
    {
        public KontinuityDB() : base("name=DefaultConnection") 
        {
            Database.Log = sql => System.Diagnostics.Debug.Write(sql);
        }

        public DbSet<DeclineReason> DeclineReasons { get; set; }
        public DbSet<OrderTimeEvent> OrderTimeEvents { get; set; }

        public DbSet<TriggerEmailTable> TriggerEmailTables { get; set; }
        public DbSet<Balancer> Balancers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Partial> Partials { get; set; }
        public DbSet<Postback> Postbacks { get; set; }
        public DbSet<Processor> Processors { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductEvent> ProductEvents { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<BalancerProcessor> BalancerProcessors { get; set; }
        public DbSet<FulfillmentProvider> FulfillmentProviders { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<VariantExtraField> VariantExtraFields { get; set; }
        public DbSet<ExportTemplate> ExportTemplate { get; set; }

        public DbSet<ExportTemplateFields> ExportTemplateFields { get; set; }
        public DbSet<OrderNote> OrderNotes { get; set; }
        //public DbSet<DeclineSalvage> DeclineSalvages { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupsInRoles> UserGroupsInRoles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<SmtpServer> SmtpServers { get; set; }
        public DbSet<RMAReason> RMAReasons { get; set; }
        public DbSet<ReturnProfile> ReturnProfiles { get; set; }

        public DbSet<ShippingCategory> ShippingCategories { get; set; }
        
        public DbSet<AutoResponderProvider> AutoResponderProviders { get; set; }
        public DbSet<KLog> KLogs { get; set; }

        public DbSet<PostBackUrl> PostBackUrls { get; set; }
        //public DbSet<Provider> Providers { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PrepaidInfo> PrepaidInfos { get; set; }
        public DbSet<FormGeneration> FormGenerations { get; set; }

        public DbSet<SalvageProfile> SalvageProfiles { get; set; }
        public DbSet<DeclineType> DeclineTypes { get; set; }
        public DbSet<ProductSalvage> ProductSalvages { get; set; }
        public DbSet<TestCardNumber> TestCardNumbers { get; set; }
        public DbSet<TaxProfile> TaxProfiles { get; set; }
        public DbSet<TaxRule> TaxRules { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Block> Blocks { get; set; }

        public DbSet<BlackList> BlackList { get; set; }
        public DbSet<ProductPaymentType> ProductPaymentType { get; set; }
        public DbSet<StandardTimeZone> TimeZones { get; set; }

        public DbSet<ExportFields> ExportFields { get; set; }
        public DbSet<ProcessorCascade> ProcessorCascade { get; set; }

        public DbSet<Widgets> Widgets { get; set; }

        public DbSet<PrepaidCard> PrepaidCards { get; set; }

        public DbSet<TransactionViaPrepaidCardQueue> TransactionViaPrepaidCardQueues { get; set; }

        public DbSet<PrepaidCardTransactionQueue> PrepaidCardTransactionQueues { get; set; }

        public DbSet<TransactionQueue> TransactionQueues { get; set; }

        public DbSet<TransactionQueueMaster> TransactionQueueMasters { get; set; }

        public DbSet<webpages_Membership> WebpagesMembership { get; set; }

        public DbSet<Webpages_UsersInRoles> WebpagesUsersInRoles { get; set; }

        public DbSet<webpages_Roles> webpagesRoles { get; set; }

    }
}
