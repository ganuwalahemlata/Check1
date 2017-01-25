using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using KontinuityCRM.Models;
using TrackerEnabledDbContext.Common.Models;

namespace KontinuityCRM.Helpers
{
    public interface IUnitOfWork
    {
        DbCommand CreateCommand();
        IRepository<OrderTimeEvent> OrderTimeEventRepository { get; }
        IRepository<DeclineReason> DeclineReasonRepository { get; }
        IRepository<Transaction> TransactionRepository { get; }

        IRepository<TransactionViaPrepaidCardQueue> TransactionViaPrepaidCardQueueRepository { get; }

        IRepository<PrepaidCard> PrepaidCardRepository { get; }

        IRepository<PrepaidCardTransactionQueue> PrepaidCardTransactionQueueRepository { get; }

        IRepository<TransactionQueueMaster> TransactionQueueMasterRepository { get; }

        IRepository<TransactionQueue> TransactionQueueRepository { get; }




        IRepository<SmtpServer> SmtpServerRepository { get; }
        IRepository<KLog> KLogRepository { get; }
        IRepository<AutoResponderProvider> AutoResponderProviderRepository { get; }
        IRepository<PostBackUrl> PostBackUrlRepository { get; }
        IRepository<RMAReason> RMAReasonRepository { get; }
        IRepository<ReturnProfile> ReturnProfileRepository { get; }
        IRepository<ShippingCategory> ShippingCategoryRepository { get; }
        IRepository<UserProfile> UserProfileRepository { get; }
        IRepository<FulfillmentProvider> FulfillmentProviderRepository { get; }
        IRepository<Country> CountryRepository { get; }
        IRepository<UserGroup> UserGroupRepository { get; }
        IRepository<UserGroupsInRoles> UserGroupsInRolesRepository { get; }
        IRepository<OrderNote> OrderNoteRepository { get; }
        //IRepository<DeclineSalvage> DeclineSalvageRepository { get; }
        IRepository<ProductEvent> ProductEventRepository { get; }
        IRepository<ProductVariant> ProductVariantRepository { get; }
        IRepository<VariantExtraField> VariantExtraFieldRepository { get; }
        IRepository<ShippingMethod> ShippingMethodRepository { get; }
        IRepository<BalancerProcessor> BalancerProcessorRepository { get; }
        IRepository<Partial> PartialRepository { get; }
        IRepository<Postback> PostbackRepository { get; }
        IRepository<Processor> ProcessorRepository { get; }
        IRepository<OrderProduct> OrderProductRepository { get; }
        IRepository<EmailTemplate> EmailTemplateRepository { get; }
        IRepository<Event> EventRepository { get; }
        IRepository<Gateway> GatewayRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<Balancer> BalancerRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<PrepaidInfo> PrepaidInfoRepository { get; }
        IRepository<FormGeneration> FormGenerationRepository { get; }
        IRepository<AuditLog> AuditLogRepository { get; }
        IRepository<SalvageProfile> SalvageProfileRepository { get; }
        IRepository<DeclineType> DeclineTypeRepository { get; }
        IRepository<ProductSalvage> ProductSalvageRepository { get; }
        IRepository<TestCardNumber> TestCardNumberRepository { get; }
        IRepository<TaxProfile> TaxProfileRepository { get; }
        IRepository<TaxRule> TaxRuleRepository { get; }
        IRepository<Location> LocationRepository { get; }
        IRepository<Block> BlockRepository { get; }
        IRepository<ExportTemplate> ExportTemplateRepository { get; }
        IRepository<ExportTemplateFields> ExportTemplateFieldsRepository { get; }
        IRepository<BlackList> BlackListRepository { get; }
        IRepository<PaymentTypes> PaymentTypesRepository { get; }
        IRepository<ProductPaymentType> ProductPaymentTypeRepository { get; }
        IRepository<StandardTimeZone> TimeZoneRepository { get; }
        IRepository<ExportFields> ExportFieldsForCSV { get; }
        IRepository<ProcessorCascade> ProcessorCascadeRepository { get; }

        IRepository<TriggerEmailTable> TriggerEmailTableRepository { get; }

        IRepository<Widgets> WidgetsRepository { get; }
        int Save();
        int Save(object userid);
        DbRawSqlQuery<T> SqlQuery<T>(string query, params object[] parameters);
        DbRawSqlQuery SqlQuery(Type type, string query, params object[] parameters);

        void Dispose();
    }
}
