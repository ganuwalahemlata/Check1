using System;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System.Data.Entity.Validation;
using TrackerEnabledDbContext.Common.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Common;

namespace KontinuityCRM.Helpers
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IContinuityDbContext context;

        private IRepository<Product> productRepository;
        private IRepository<Order> orderRepository;
        private IRepository<Balancer> balancerRepository;
        private IRepository<Category> categoryRepository;
        private IRepository<Customer> customerRepository;
        private IRepository<EmailTemplate> emailTemplateRepository;
        private IRepository<Event> eventRepository;
        private IRepository<Gateway> gatewayRepository;
        private IRepository<Partial> partialRepository;
        private IRepository<Postback> postbackRepository;
        private IRepository<Processor> processorRepository;
        private IRepository<OrderProduct> orderProductRepository;
        private IRepository<ProductVariant> productVariantRepository;
        private IRepository<VariantExtraField> variantExtraFieldRepository;
        private IRepository<ProductEvent> productEventRepository;
        private IRepository<ShippingMethod> shippingMethodRepository;
        private IRepository<BalancerProcessor> balancerProcessorRepository;
        private IRepository<FulfillmentProvider> fulfillmentProviderRepository;
        private IRepository<Country> countryRepository;
        private IRepository<OrderNote> orderNoteRepository;
        private IRepository<SalvageProfile> salvageProfileRepository;
        private IRepository<DeclineType> declineTypeRepository;
        private IRepository<ProductSalvage> productSalvageRepository;
        private IRepository<RMAReason> rmaReasonRepository;
        private IRepository<ReturnProfile> returnProfileRepository;
        private IRepository<UserProfile> userProfileRepository;
        private IRepository<ShippingCategory> shippingCategoryRepository;
        private IRepository<SmtpServer> smtpServerRepository;
        private IRepository<KLog> kLogRepository;
        private IRepository<AutoResponderProvider> autoResponderProviderRepository;
        private IRepository<PostBackUrl> postBackUrlRepository;
        private IRepository<Transaction> transactionRepository;
        private IRepository<DeclineReason> declineReasonRepository;
        private IRepository<PrepaidInfo> prepaidInfoRepository;
        private IRepository<FormGeneration> formGenerationRepository;
        private IRepository<AuditLog> auditLogRepository;
        private IRepository<OrderTimeEvent> orderTimeEventRepository;
        private IRepository<TestCardNumber> testCardNumberRepository;
        private IRepository<TaxProfile> taxProfileRepository;
        private IRepository<TaxRule> taxRuleRepository;
        private IRepository<Block> blockRepository;
        private IRepository<Location> locationRepository;
        private IRepository<ExportTemplate> exportTemplateRepository;
        private IRepository<ExportTemplateFields> exportTemplateFieldsRepository;
        private IRepository<UserGroup> userGroupRepository;
        private IRepository<UserGroupsInRoles> userGroupsInRolesRepository;
        private IRepository<PaymentTypes> paymentTypesRepository;
        private IRepository<ProductPaymentType> productPaymentTypeRepository;
        private IRepository<BlackList> blackListRepository;
        private IRepository<StandardTimeZone> timeZoneRepository;
        private IRepository<ExportFields> exporFieldsForCSVRepository;
        private IRepository<ProcessorCascade> processorCascadeRepository;
        private IRepository<Widgets> widgetsRepository;
        private IRepository<PrepaidCard> prepaidCardRepository;
        private IRepository<PrepaidCardTransactionQueue> prepaidCardTransactionQueueRepository;
        private IRepository<TransactionQueueMaster> transactionQueueMasterRepository;
        private IRepository<TransactionQueue> transactionQueueRepository;
        private IRepository<TriggerEmailTable> triggerEmailTableRepository;

        private IRepository<TransactionViaPrepaidCardQueue> transactionViaPrepaidCardQueueRepository;

        public IRepository<ProcessorCascade> ProcessorCascadeRepository
        {
            get
            {
                if (this.processorCascadeRepository == null)
                {
                    this.processorCascadeRepository = new GenericRepository<ProcessorCascade>(context);
                }
                return processorCascadeRepository;
            }
        }

        public IRepository<Location> LocationRepository
        {
            get
            {
                if (this.locationRepository == null)
                {
                    this.locationRepository = new GenericRepository<Location>(context);
                }
                return locationRepository;
            }
        }

        public IRepository<Block> BlockRepository
        {
            get
            {
                if (this.blockRepository == null)
                {
                    this.blockRepository = new GenericRepository<Block>(context);
                }
                return blockRepository;
            }
        }
        public IRepository<TaxRule> TaxRuleRepository
        {
            get
            {
                if (this.taxRuleRepository == null)
                {
                    this.taxRuleRepository = new GenericRepository<TaxRule>(context);
                }
                return taxRuleRepository;
            }
        }

        public IRepository<TaxProfile> TaxProfileRepository
        {
            get
            {
                if (this.taxProfileRepository == null)
                {
                    this.taxProfileRepository = new GenericRepository<TaxProfile>(context);
                }
                return taxProfileRepository;
            }
        }

        public IRepository<TestCardNumber> TestCardNumberRepository
        {
            get
            {
                if (this.testCardNumberRepository == null)
                {
                    this.testCardNumberRepository = new GenericRepository<TestCardNumber>(context);
                }
                return testCardNumberRepository;
            }
        }
        public IRepository<OrderTimeEvent> OrderTimeEventRepository
        {
            get
            {
                if (this.orderTimeEventRepository == null)
                {
                    this.orderTimeEventRepository = new GenericRepository<OrderTimeEvent>(context);
                }
                return orderTimeEventRepository;
            }
        }
        public IRepository<AuditLog> AuditLogRepository
        {
            get
            {
                if (this.auditLogRepository == null)
                {
                    this.auditLogRepository = new GenericRepository<AuditLog>(context);
                }
                return auditLogRepository;
            }
        }

        public IRepository<Transaction> TransactionRepository
        {
            get
            {
                if (this.transactionRepository == null)
                {
                    this.transactionRepository = new GenericRepository<Transaction>(context);
                }
                return transactionRepository;
            }
        }

        public IRepository<SmtpServer> SmtpServerRepository
        {
            get
            {
                if (this.smtpServerRepository == null)
                {
                    this.smtpServerRepository = new GenericRepository<SmtpServer>(context);
                }
                return smtpServerRepository;
            }
        }

        public IRepository<KLog> KLogRepository
        {
            get
            {
                if (this.kLogRepository == null)
                {
                    this.kLogRepository = new GenericRepository<KLog>(context);
                }
                return kLogRepository;
            }
        }

        public IRepository<AutoResponderProvider> AutoResponderProviderRepository
        {
            get
            {
                if (this.autoResponderProviderRepository == null)
                {
                    this.autoResponderProviderRepository = new GenericRepository<AutoResponderProvider>(context);
                }
                return autoResponderProviderRepository;
            }
        }

        public IRepository<PostBackUrl> PostBackUrlRepository
        {
            get
            {
                if (this.postBackUrlRepository == null)
                {
                    this.postBackUrlRepository = new GenericRepository<PostBackUrl>(context);
                }
                return postBackUrlRepository;
            }
        }

        public IRepository<RMAReason> RMAReasonRepository
        {
            get
            {
                if (this.rmaReasonRepository == null)
                {
                    this.rmaReasonRepository = new GenericRepository<RMAReason>(context);
                }
                return rmaReasonRepository;
            }
        }

        public IRepository<ReturnProfile> ReturnProfileRepository
        {
            get
            {
                if (this.returnProfileRepository == null)
                {
                    this.returnProfileRepository = new GenericRepository<ReturnProfile>(context);
                }
                return returnProfileRepository;
            }
        }

        public IRepository<ShippingCategory> ShippingCategoryRepository
        {
            get
            {
                if (this.shippingCategoryRepository == null)
                {
                    this.shippingCategoryRepository = new GenericRepository<ShippingCategory>(context);
                }
                return shippingCategoryRepository;
            }
        }

        public IRepository<UserProfile> UserProfileRepository
        {
            get
            {
                if (this.userProfileRepository == null)
                {
                    this.userProfileRepository = new GenericRepository<UserProfile>(context);
                }
                return userProfileRepository;
            }
        }

        public IRepository<FulfillmentProvider> FulfillmentProviderRepository
        {
            get
            {
                if (this.fulfillmentProviderRepository == null)
                {
                    this.fulfillmentProviderRepository = new GenericRepository<FulfillmentProvider>(context);
                }
                return fulfillmentProviderRepository;
            }
        }

        public IRepository<Country> CountryRepository
        {
            get
            {
                if (this.countryRepository == null)
                {
                    this.countryRepository = new GenericRepository<Country>(context);
                }
                return countryRepository;
            }
        }

        public IRepository<OrderNote> OrderNoteRepository
        {
            get
            {
                if (this.orderNoteRepository == null)
                {
                    this.orderNoteRepository = new GenericRepository<OrderNote>(context);
                }
                return orderNoteRepository;
            }
        }

        public IRepository<SalvageProfile> SalvageProfileRepository
        {
            get
            {
                if (this.salvageProfileRepository == null)
                {
                    this.salvageProfileRepository = new GenericRepository<SalvageProfile>(context);
                }
                return salvageProfileRepository;
            }
        }

        public IRepository<DeclineType> DeclineTypeRepository
        {
            get
            {
                if (this.declineTypeRepository == null)
                {
                    this.declineTypeRepository = new GenericRepository<DeclineType>(context);
                }
                return declineTypeRepository;
            }
        }

        public IRepository<ProductSalvage> ProductSalvageRepository
        {
            get
            {
                if (this.productSalvageRepository == null)
                {
                    this.productSalvageRepository = new GenericRepository<ProductSalvage>(context);
                }
                return productSalvageRepository;
            }
        }

        public IRepository<ProductEvent> ProductEventRepository
        {
            get
            {
                if (this.productEventRepository == null)
                {
                    this.productEventRepository = new GenericRepository<ProductEvent>(context);
                }
                return productEventRepository;
            }
        }

        public IRepository<ProductVariant> ProductVariantRepository
        {
            get
            {
                if (this.productVariantRepository == null)
                {
                    this.productVariantRepository = new GenericRepository<ProductVariant>(context);
                }
                return productVariantRepository;
            }
        }

        public IRepository<VariantExtraField> VariantExtraFieldRepository
        {
            get
            {
                if (this.variantExtraFieldRepository == null)
                {
                    this.variantExtraFieldRepository = new GenericRepository<VariantExtraField>(context);
                }
                return variantExtraFieldRepository;
            }
        }

        public IRepository<ShippingMethod> ShippingMethodRepository
        {
            get
            {
                if (this.shippingMethodRepository == null)
                {
                    this.shippingMethodRepository = new GenericRepository<ShippingMethod>(context);
                }
                return shippingMethodRepository;
            }
        }

        public IRepository<BalancerProcessor> BalancerProcessorRepository
        {
            get
            {
                if (this.balancerProcessorRepository == null)
                {
                    this.balancerProcessorRepository = new GenericRepository<BalancerProcessor>(context);
                }
                return balancerProcessorRepository;
            }

        }

        public IRepository<Partial> PartialRepository
        {
            get
            {
                if (this.partialRepository == null)
                {
                    this.partialRepository = new GenericRepository<Partial>(context);
                }
                return partialRepository;
            }
        }

        public IRepository<Postback> PostbackRepository
        {
            get
            {
                if (this.postbackRepository == null)
                {
                    this.postbackRepository = new GenericRepository<Postback>(context);
                }
                return postbackRepository;
            }
        }

        public IRepository<Processor> ProcessorRepository
        {
            get
            {
                if (this.processorRepository == null)
                {
                    this.processorRepository = new GenericRepository<Processor>(context);
                }
                return processorRepository;
            }
        }

        public IRepository<OrderProduct> OrderProductRepository
        {
            get
            {
                if (this.orderProductRepository == null)
                {
                    this.orderProductRepository = new GenericRepository<OrderProduct>(context);
                }
                return orderProductRepository;
            }
        }

        public IRepository<EmailTemplate> EmailTemplateRepository
        {
            get
            {
                if (this.emailTemplateRepository == null)
                {
                    this.emailTemplateRepository = new GenericRepository<EmailTemplate>(context);
                }
                return emailTemplateRepository;
            }
        }

        public IRepository<Event> EventRepository
        {
            get
            {
                if (this.eventRepository == null)
                {
                    this.eventRepository = new GenericRepository<Event>(context);
                }
                return eventRepository;
            }
        }

        public IRepository<Gateway> GatewayRepository
        {
            get
            {
                if (this.gatewayRepository == null)
                {
                    this.gatewayRepository = new GenericRepository<Gateway>(context);
                }
                return gatewayRepository;
            }
        }

        public IRepository<Product> ProductRepository
        {
            get
            {
                if (this.productRepository == null)
                {
                    this.productRepository = new GenericRepository<Product>(context);
                }
                return productRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (this.orderRepository == null)
                {
                    this.orderRepository = new GenericRepository<Order>(context);
                }
                return orderRepository;
            }

        }

        public IRepository<Balancer> BalancerRepository
        {
            get
            {
                if (this.balancerRepository == null)
                {
                    this.balancerRepository = new GenericRepository<Balancer>(context);
                }
                return balancerRepository;
            }

        }

        public IRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(context);
                }
                return categoryRepository;
            }

        }

        public IRepository<Customer> CustomerRepository
        {
            get
            {
                if (this.customerRepository == null)
                {
                    this.customerRepository = new GenericRepository<Customer>(context);
                }
                return customerRepository;
            }

        }

        public IRepository<PrepaidInfo> PrepaidInfoRepository
        {
            get
            {
                if (this.prepaidInfoRepository == null)
                {
                    this.prepaidInfoRepository = new GenericRepository<PrepaidInfo>(context);
                }
                return prepaidInfoRepository;
            }

        }

        public IRepository<FormGeneration> FormGenerationRepository
        {
            get
            {
                if (this.formGenerationRepository == null)
                {
                    this.formGenerationRepository = new GenericRepository<FormGeneration>(context);
                }
                return formGenerationRepository;
            }

        }
        public IRepository<ExportTemplate> ExportTemplateRepository
        {
            get
            {
                if (this.exportTemplateRepository == null)
                {
                    this.exportTemplateRepository = new GenericRepository<ExportTemplate>(context);
                }
                return exportTemplateRepository;
            }
        }

        public IRepository<ExportTemplateFields> ExportTemplateFieldsRepository
        {
            get
            {
                if (this.exportTemplateFieldsRepository == null)
                {
                    this.exportTemplateFieldsRepository = new GenericRepository<ExportTemplateFields>(context);
                }
                return exportTemplateFieldsRepository;
            }
        }

        public IRepository<UserGroup> UserGroupRepository
        {
            get
            {
                if (this.userGroupRepository == null)
                {
                    this.userGroupRepository = new GenericRepository<UserGroup>(context);
                }
                return userGroupRepository;
            }
        }

        public IRepository<UserGroupsInRoles> UserGroupsInRolesRepository
        {
            get
            {
                if (this.userGroupsInRolesRepository == null)
                {
                    this.userGroupsInRolesRepository = new GenericRepository<UserGroupsInRoles>(context);
                }
                return userGroupsInRolesRepository;
            }
        }



        public IRepository<BlackList> BlackListRepository
        {
            get
            {
                if (this.blackListRepository == null)
                {
                    this.blackListRepository = new GenericRepository<BlackList>(context);
                }
                return blackListRepository;
            }

        }

        public IRepository<PaymentTypes> PaymentTypesRepository
        {
            get
            {
                if (this.paymentTypesRepository == null)
                {
                    this.paymentTypesRepository = new GenericRepository<PaymentTypes>(context);
                }
                return paymentTypesRepository;
            }

        }
        public IRepository<ProductPaymentType> ProductPaymentTypeRepository
        {
            get
            {
                if (this.productPaymentTypeRepository == null)
                {
                    this.productPaymentTypeRepository = new GenericRepository<ProductPaymentType>(context);
                }
                return productPaymentTypeRepository;
            }

        }
        public IRepository<StandardTimeZone> TimeZoneRepository
        {
            get
            {
                if (this.productPaymentTypeRepository == null)
                {
                    this.timeZoneRepository = new GenericRepository<StandardTimeZone>(context);
                }
                return timeZoneRepository;
            }

        }

        public IRepository<ExportFields> ExportFieldsForCSV
        {

            get
            {
                if (this.exporFieldsForCSVRepository == null)
                {
                    this.exporFieldsForCSVRepository = new GenericRepository<ExportFields>(context);
                }

                return exporFieldsForCSVRepository;
            }
        }
        public IRepository<Widgets> WidgetsRepository
        {
            get
            {
                if (this.widgetsRepository == null)
                {
                    this.widgetsRepository = new GenericRepository<Widgets>(context);
                }
                return widgetsRepository;
            }
        }

        public IRepository<TransactionViaPrepaidCardQueue> TransactionViaPrepaidCardQueueRepository
        {
            get
            {
                if (this.transactionViaPrepaidCardQueueRepository == null)
                {
                    this.transactionViaPrepaidCardQueueRepository = new GenericRepository<TransactionViaPrepaidCardQueue>(context);
                }
                return transactionViaPrepaidCardQueueRepository;
            }
        }

        public IRepository<PrepaidCard> PrepaidCardRepository
        {
            get
            {
                if (this.prepaidCardRepository == null)
                {
                    this.prepaidCardRepository = new GenericRepository<PrepaidCard>(context);
                }
                return prepaidCardRepository;
            }
        }

        public IRepository<PrepaidCardTransactionQueue> PrepaidCardTransactionQueueRepository
        {
            get
            {
                if (this.prepaidCardTransactionQueueRepository == null)
                {
                    this.prepaidCardTransactionQueueRepository = new GenericRepository<PrepaidCardTransactionQueue>(context);
                }
                return prepaidCardTransactionQueueRepository;
            }
        }

        public IRepository<TransactionQueueMaster> TransactionQueueMasterRepository
        {
            get
            {
                if (this.transactionQueueMasterRepository == null)
                {
                    this.transactionQueueMasterRepository = new GenericRepository<TransactionQueueMaster>(context);
                }
                return transactionQueueMasterRepository;
            }
        }

        public IRepository<TransactionQueue> TransactionQueueRepository
        {
            get
            {
                if (this.transactionQueueRepository == null)
                {
                    this.transactionQueueRepository = new GenericRepository<TransactionQueue>(context);
                }
                return transactionQueueRepository;
            }
        }

        public IRepository<TriggerEmailTable> TriggerEmailTableRepository
        {
            get
            {
                if (this.triggerEmailTableRepository == null)
                {
                    this.triggerEmailTableRepository = new GenericRepository<TriggerEmailTable>(context);
                }
                return triggerEmailTableRepository;
            }
        }



        //private IRepository<PaymentType> paymentTypeRepository;
        //private IRepository<ProductPaymentType> productPaymentTypeRepository;

        public UnitOfWork(IContinuityDbContext continuityDbContext)
        {
            context = continuityDbContext;
        }

        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {
                string s = null, t = null;
                foreach (var eve in e.EntityValidationErrors)
                {
                    s += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        t += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(e.Message, new Exception(s + "\r\n" + t));
            }
        }

        private bool disposed = false;

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Save(object userid)
        {
            try
            {
                return context.SaveChanges(userid);
            }

            catch (DbEntityValidationException e)
            {
                string s = null, t = null;
                foreach (var eve in e.EntityValidationErrors)
                {
                    s += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        t += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(e.Message, new Exception(s + "\r\n" + t));
            }

            //return context.SaveChanges(userid);
        }

        public IRepository<DeclineReason> DeclineReasonRepository
        {
            get
            {
                if (this.declineReasonRepository == null)
                {
                    this.declineReasonRepository = new GenericRepository<DeclineReason>(context);
                }
                return declineReasonRepository;
            }
        }

        public DbRawSqlQuery<T> SqlQuery<T>(string query, params object[] parameters)
        {
            return context.Database.SqlQuery<T>(query, parameters);
        }

        public DbRawSqlQuery SqlQuery(Type type, string query, params object[] parameters)
        {
            return context.Database.SqlQuery(type, query, parameters);
        }

        public DbCommand CreateCommand()
        {
            return context.Database.Connection.CreateCommand();
        }

        //public DbParameter CreateCommand(string name, System.Data.DbType type, object value)
        //{
        //    var param = context.Database.Connection.CreateCommand().CreateParameter();
        //    param.DbType = type;
        //    param.ParameterName = name;
        //    param.Value = value;

        //    return param;
        //}
    }
}
