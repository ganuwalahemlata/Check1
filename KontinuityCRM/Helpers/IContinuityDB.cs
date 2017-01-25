using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KontinuityCRM.Models;

namespace KontinuityCRM.Helpers
{
    public interface IContinuityDbContext : IDisposable
    {
        DbSet<DeclineReason> DeclineReasons { get; set; }
        DbSet<OrderTimeEvent> OrderTimeEvents { get; set; }

        DbSet<TriggerEmailTable> TriggerEmailTables { get; set; }
        DbSet<Balancer> Balancers { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<EmailTemplate> EmailTemplates { get; set; }
        DbSet<Event> Events { get; set; }
        DbSet<Gateway> Gateways { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderProduct> OrderProducts { get; set; }
        DbSet<Partial> Partials { get; set; }
        DbSet<Postback> Postbacks { get; set; }
        DbSet<Processor> Processors { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductEvent> ProductEvents { get; set; }
        DbSet<ProductVariant> ProductVariants { get; set; }
        DbSet<ShippingMethod> ShippingMethods { get; set; }
        DbSet<BalancerProcessor> BalancerProcessors { get; set; }
        DbSet<FulfillmentProvider> FulfillmentProviders { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<VariantExtraField> VariantExtraFields { get; set; }
        DbSet<ExportTemplate> ExportTemplate { get; set; }

        DbSet<ExportTemplateFields> ExportTemplateFields { get; set; }
        DbSet<OrderNote> OrderNotes { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
        DbSet<UserGroupsInRoles> UserGroupsInRoles { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<SmtpServer> SmtpServers { get; set; }
        DbSet<RMAReason> RMAReasons { get; set; }
        DbSet<ReturnProfile> ReturnProfiles { get; set; }
        DbSet<ShippingCategory> ShippingCategories { get; set; }
        DbSet<AutoResponderProvider> AutoResponderProviders { get; set; }
        DbSet<KLog> KLogs { get; set; }
        DbSet<PostBackUrl> PostBackUrls { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<PrepaidInfo> PrepaidInfos { get; set; }
        DbSet<FormGeneration> FormGenerations { get; set; }
        DbSet<SalvageProfile> SalvageProfiles { get; set; }
        DbSet<DeclineType> DeclineTypes { get; set; }
        DbSet<ProductSalvage> ProductSalvages { get; set; }
        DbSet<TestCardNumber> TestCardNumbers { get; set; }
        DbSet<TaxProfile> TaxProfiles { get; set; }
        DbSet<TaxRule> TaxRules { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Block> Blocks { get; set; }
        DbSet<BlackList> BlackList { get; set; }
        DbSet<ProductPaymentType> ProductPaymentType { get; set; }


        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbContextConfiguration Configuration { get; }
        Database Database { get; }

        int SaveChanges();
        int SaveChanges(object userName);
    }
}
