using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using TrackerEnabledDbContext.Common.Models;

namespace KontinuityCRM.Helpers
{
    public interface IKontinuityCRMRepo : IDisposable
    {      

        Category FindCategory(int id);

        IQueryable<Category> Categories();

        void EditCategory(Category category);

        void RemoveCategory(Category category);

        void AddCategory(Category category);

        IQueryable<ShippingMethod> ShippingMethods();

        void AddShippingMethod(ShippingMethod sm);

        ShippingMethod FindShippingMethod(int id);

        void EditShippingMethod(ShippingMethod sm);

        void RemoveShippingMethod(ShippingMethod sm);

        IQueryable<FulfillmentProvider> FulfillmentProviders();

        void AddFulfillmentProvider(FulfillmentProvider fp);

        FulfillmentProvider FindFulfillmentProvider(int id);

        void EditFulfillmentProvider(FulfillmentProvider fp);

        void RemoveFulfillmentProvider(FulfillmentProvider entity);

        IQueryable<Product> Products();

        Product FindProduct(int id);

        void AddProduct(Product product);

        //void EditProduct(Product product);

        void RemoveProduct(Product product);

        IQueryable<Partial> Partials();

        Partial FindPartial(int id);

        void AddPartial(Partial partial);

        void EditPartial(Partial partial);

        void RemovePartial(Partial partial);

        IQueryable<Customer> Customers();

        Customer FindCustomer(int id);

        void AddCustomer(Customer customer);

        void EditCustomer(Customer customer);

        void RemoveCustomer(Customer customer);

        EmailTemplate FindEmailTemplate(int id);

        void AddEmailTemplate(EmailTemplate email);

        void EditEmailTemplate(EmailTemplate email);

        void RemoveEmailTemplate(EmailTemplate email);

        IQueryable<EmailTemplate> EmailTemplates();

       IQueryable<Balancer> Balancers();

       Balancer FindBalancer(int id);

       void AddBalancer(Balancer balancer);

       void EditBalancer(Balancer balancer);

       void RemoveBalancer(Balancer balancer);

       IQueryable<Gateway> Gateways();

       Gateway FindGateway(int id);

       void AddGateway(Gateway gateway);

       void EditGateway(Gateway gateway);

       void RemoveGateway(Gateway gateway);

       IQueryable<Processor> Processors();

       Processor FindProcessor(int id);

       void AddProcessor(Processor processor);

       void EditProcessor(Processor processor);

       void RemoveProcessor(Processor processor);

       IQueryable<Postback> Postbacks();

       Postback FindPostback(int id);

       void AddPostback(Postback postback);

       void EditPostback(Postback postback);

       void RemovePostback(Postback postback);

       IQueryable<Event> Events();

       Event FindEvent(int id);

       void AddEvent(Event e);

       void EditEvent(Event e);

       void RemoveEvent(Event e);

       IQueryable<ProductVariant> ProductVariants();

       ProductVariant FindProductVariant(int id);

       void AddProductVariant(ProductVariant pv);

       void EditProductVariant(ProductVariant pv);

       void RemoveProductVariant(ProductVariant pv);

       IQueryable<Order> Orders();

       void AddOrder(Order order);

       Order FindOrder(int id);

       void EditOrder(Order order);

       void RemoveOrder(Order order);

       //IQueryable<ProductCategory> ProductCategories();

       //ProductCategory FindProductCategory(int productid, int categoryid);

       //ProductCategory FindProductCategory(int productcategoryid);

       //void AddProductCategory(ProductCategory pc);

       //void EditProductCategory(ProductCategory pc);

       //void RemoveProductCategory(ProductCategory pc);

       IQueryable<OrderProduct> OrderProducts();

       OrderProduct FindOrderProduct(int id);

       OrderProduct FindOrderProduct(int orderid, int productid);

       void AddOrderProduct(OrderProduct op);

       void EditOrderProduct(OrderProduct op);

       void RemoveOrderProduct(OrderProduct op);

       IQueryable<BalancerProcessor> BalancerProcessors();

       BalancerProcessor FindBalancerProcessor(int id);

       BalancerProcessor FindBalancerProcessor(int balancerid, int processorid);

       void AddBalancerProcessor(BalancerProcessor bp);

       void EditBalancerProcessor(BalancerProcessor bp);

       void RemoveBalancerProcessor(BalancerProcessor bp);

       IQueryable<ProductEvent> ProductEvents();

       ProductEvent FindProductEvent(int id);

       ProductEvent FindProductEvent(int productid, int eventid);

       void AddProductEvent(ProductEvent pe);

       void EditProductEvent(ProductEvent pe);

       void RemoveProductEvent(ProductEvent pe);

       IQueryable<Country> Countries();

       IQueryable<AuditLog> AuditLogs { get; }

       //void AddCustomField(CustomField customfield);

       AuditLog FindAuditLog(int id);

       //void EditCustomField(CustomField cf);

       //void RemoveCustomField(CustomField cf);

       //void AddCustomFieldValue(CustomFieldValue customFieldValue);

       //IQueryable<CustomFieldValue> CustomFieldValues();

       //void RemoveCustomFields(IQueryable<CustomFieldValue> range);

       int SaveChanges();

       void AddVariantExtraField(VariantExtraField variantExtraField);

       IQueryable<VariantExtraField> VariantExtraFields();

       void RemoveExtraFields(ProductVariant pv);


       //DeclineSalvage FindDeclineSalvage(int id);

       //void EditDeclineSalvage(DeclineSalvage ds);

       //void AddDeclineSalvage(DeclineSalvage declineSalvage);

       //IQueryable<DeclineSalvage> DeclineSalvages();

       void AddOrderNote(OrderNote ordernote);

       IQueryable<OrderNote> OrderNotes();

       IQueryable<UserProfile> UserProfiles();

       UserProfile FindUserProfile(int id);

       void EditUserProfile(UserProfile user);

       void RemoveUserProfile(UserProfile user);

       IQueryable<SmtpServer> SmtpServers();

       void AddSmtpServer(SmtpServer server);

       void RemoveSmtpServer(SmtpServer server);

       void EditSmtpServer(SmtpServer server);

       SmtpServer FindSmtpServer(int id);

       AutoResponderProvider FindAutoResponderProvider(int id);

       IQueryable<AutoResponderProvider> AutoResponderProviders { get; }

       void AddAutoResponderProvider(AutoResponderProvider responder);

       void RemoveAutoResponderProvider(AutoResponderProvider responder);

       void EditAutoResponderProvider(AutoResponderProvider responder);

       void AddLog(KLog klog);

       DbSet<ShippingCategory> ShippingCategories { get; }

       void AddShippingCategory(ShippingCategory shippingcategory);

       ShippingCategory FindShippingCategory(int id);

       void RemoveShippingCategory(ShippingCategory shippingcategory);

       void EditShippingCategory(ShippingCategory shippingcategory);

       DbSet<PostBackUrl> PostBackUrls { get;  }

       void RemovePostbackUrls(IEnumerable<PostBackUrl> pburls);

       void AddPostbackUrl(PostBackUrl url);

       
    }

    public class EFKontinuityCRMRepo : IKontinuityCRMRepo
    {
        private KontinuityDB db;

        public EFKontinuityCRMRepo()
        {
            db = new KontinuityDB();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        /*===================Orders====================*/

        public void AddOrder(Order order)
        {
            try
            {
                db.Orders.Add(order);
                db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    string s, t;
                    s = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        t = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw e;
            }
        }

        public IQueryable<Order> Orders()
        {
            return db.Orders;
        }

        public Order FindOrder(int id)
        {
            return db.Orders.Find(id);
        }

        public int SaveChanges()
        {
            return db.SaveChanges(/* WebMatrix.WebData.WebSecurity.CurrentUserId */); 
        }

        public void EditOrder(Order order)
        {
            db.Entry(order).State = EntityState.Modified;

            //try
            //{ 
            //    db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
            //}           

            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        string s, t;
            //        s = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);

            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            t = string.Format("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }

        public void RemoveOrder(Order order)
        {
            db.Orders.Remove(order);
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        /*===================Orders====================*/

        /*=================== Category ====================*/

        public Category FindCategory(int id)
        {
            return db.Categories.Find(id);
        }

        public IQueryable<Category> Categories()
        {
            return db.Categories;
        }

        public void EditCategory(Category category)
        {
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveCategory(Category category)
        {
            db.Categories.Remove(category);
            db.SaveChanges();
        }

        public void AddCategory(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
        }

        /*=================== Category ====================*/

        /*=================== ShippingMethods ====================*/

        public IQueryable<ShippingMethod> ShippingMethods()
        {
            return db.ShippingMethods;
        }

        public void AddShippingMethod(ShippingMethod sm)
        {
            db.ShippingMethods.Add(sm);
            db.SaveChanges();
        }

        public ShippingMethod FindShippingMethod(int id)
        {
            return db.ShippingMethods.Find(id);
        }

        public void EditShippingMethod(ShippingMethod sm)
        {
            db.Entry(sm).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveShippingMethod(ShippingMethod sm)
        {
            db.ShippingMethods.Remove(sm);
            db.SaveChanges();
        }

        /*=================== ShippingMethods ====================*/

        /*=================== FulfillmentProvider ====================*/

        public IQueryable<FulfillmentProvider> FulfillmentProviders()
        {
            return db.FulfillmentProviders;
        }

        public void AddFulfillmentProvider(FulfillmentProvider fp)
        {
            db.FulfillmentProviders.Add(fp);
            db.SaveChanges();
        }

        public FulfillmentProvider FindFulfillmentProvider(int id)
        {
            return db.FulfillmentProviders.Find(id);
        }

        public void EditFulfillmentProvider(FulfillmentProvider fp)
        {
            db.Entry(fp).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveFulfillmentProvider(FulfillmentProvider entity)
        {
            db.FulfillmentProviders.Remove(entity);
            db.SaveChanges();
        }

        /*=================== FulfillmentProvider ====================*/

        /*=================== Product ====================*/

        public IQueryable<Product> Products()
        {
            return db.Products;
        }

        public Product FindProduct(int id)
        {
            return db.Products.Find(id);
        }

        public void AddProduct(Product product)
        {
            db.Products.Add(product);
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        //public void EditProduct(Product product)
        //{
        //    var productindb = FindProduct(product.ProductId);

        //    db.Entry(productindb).CurrentValues.SetValues(product);

        //    // if the events is null then remove all events in the folling foreach
        //    if (product.Events == null)
        //        product.Events = new List<ProductEvent>();
            

        //    // Remove  not present productsevents relationships
        //    foreach (var peindb in productindb.Events.ToList()) // to list because we are changing the list in the foreach body
        //        if (!product.Events.Any(p => p.EventId == peindb.EventId))
        //            productindb.Events.Remove(peindb);

        //    foreach (var pe in product.Events)
        //    {
        //        var peindb = productindb.Events.SingleOrDefault(c => c.EventId == pe.EventId);
        //        if (peindb != null)
        //        {
        //            // Update courses
        //            // this event is already in 
        //        }
        //        else
        //        {
        //            // Add courses relationships
        //            //db.ProductEvents.Add(pe);
        //            productindb.Events.Add(pe);
        //        }
        //    }


        //    //db.Entry(product).State = EntityState.Modified;
        //    //db.Entry(product.Events).s
        //    db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        //}

        public void RemoveProduct(Product product)
        {
            db.Products.Remove(product);
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        /*=================== Product ====================*/

        /*=================== Partials ====================*/

        public IQueryable<Partial> Partials()
        {
            return db.Partials;
        }

        public Partial FindPartial(int id)
        {
            return db.Partials.Find(id);
        }

        public void AddPartial(Partial partial)
        {
            db.Partials.Add(partial);
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        public void EditPartial(Partial partial)
        {
            db.Entry(partial).State = EntityState.Modified;
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        public void RemovePartial(Partial partial)
        {
            db.Partials.Remove(partial);
            db.SaveChanges(WebMatrix.WebData.WebSecurity.CurrentUserId);
        }

        /*=================== Partials ====================*/

        /*=================== Customers ====================*/

        public IQueryable<Customer> Customers()
        {
            return db.Customers;
        }

        public Customer FindCustomer(int id)
        {
            return db.Customers.Find(id);
        }

        public void AddCustomer(Customer customer)
        {
            db.Customers.Add(customer);
            db.SaveChanges();
        }

        public void EditCustomer(Customer customer)
        {
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveCustomer(Customer customer)
        {
            db.Customers.Remove(customer);
            db.SaveChanges();
        }

        /*=================== Customers ====================*/

        /*=================== Email ====================*/

        public EmailTemplate FindEmailTemplate(int id)
        {
            return db.EmailTemplates.Find(id);
        }

        public void AddEmailTemplate(EmailTemplate email)
        {
            db.EmailTemplates.Add(email);
            db.SaveChanges();
        }

        public void EditEmailTemplate(EmailTemplate email)
        {
            db.Entry(email).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveEmailTemplate(EmailTemplate email)
        {
            db.EmailTemplates.Remove(email);
            db.SaveChanges();
        }

        public IQueryable<EmailTemplate> EmailTemplates()
        {
            return db.EmailTemplates;
        }

        /*=================== Email ====================*/

        /*=================== Balancers ====================*/

        public IQueryable<Balancer> Balancers()
        {
            return db.Balancers;
        }

        public Balancer FindBalancer(int id)
        {
            return db.Balancers.Find(id);
        }

        public void AddBalancer(Balancer balancer)
        {
            db.Balancers.Add(balancer);
            db.SaveChanges();
        }

        public void EditBalancer(Balancer balancer)
        {
            db.Entry(balancer).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveBalancer(Balancer balancer)
        {
            db.Balancers.Remove(balancer);
            db.SaveChanges();
        }

        /*=================== Balancers ====================*/

        /*=================== Gateways ====================*/

        public IQueryable<Gateway> Gateways()
        {
            return db.Gateways;
        }

        public Gateway FindGateway(int id)
        {
            return db.Gateways.Find(id);
        }

        public void AddGateway(Gateway gateway)
        {
            db.Gateways.Add(gateway);
            db.SaveChanges();
        }

        public void EditGateway(Gateway gateway)
        {
            db.Entry(gateway).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveGateway(Gateway gateway)
        {
            db.Gateways.Remove(gateway);
            db.SaveChanges();
        }

        /*=================== Gateways ====================*/

        /*=================== Processors ====================*/

        public IQueryable<Processor> Processors()
        {
            return db.Processors;
        }

        public Processor FindProcessor(int id)
        {
            return db.Processors.Find(id);
        }

        public void AddProcessor(Processor processor)
        {
            db.Processors.Add(processor);
            db.SaveChanges();
        }

        public void EditProcessor(Processor processor)
        {
            db.Entry(processor).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveProcessor(Processor processor)
        {
            db.Processors.Remove(processor);
            db.SaveChanges();
        }

        /*=================== Processors ====================*/

        /*=================== Postbacks ====================*/

        public IQueryable<Postback> Postbacks()
        {
            return db.Postbacks;
        }

        public Postback FindPostback(int id)
        {
            return db.Postbacks.Find(id);
        }

        public void AddPostback(Postback postback)
        {
            db.Postbacks.Add(postback);
            db.SaveChanges();
        }

        public void EditPostback(Postback postback)
        {
            db.Entry(postback).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemovePostback(Postback postback)
        {
            db.Postbacks.Remove(postback);
            db.SaveChanges();
        }

        /*=================== Postbacks ====================*/

        /*=================== Events ====================*/

        public IQueryable<Event> Events()
        {
            return db.Events;
        }

        public Event FindEvent(int id)
        {
            return db.Events.Find(id);
        }

        public void AddEvent(Event e)
        {
            db.Events.Add(e);
            db.SaveChanges();
        }

        public void EditEvent(Event e)
        {
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveEvent(Event e)
        {
            db.Events.Remove(e);
            db.SaveChanges();
        }

        /*=================== Events ====================*/

        /*=================== ProductVariants ====================*/

        public IQueryable<ProductVariant> ProductVariants()
        {
            return db.ProductVariants;
        }

        public ProductVariant FindProductVariant(int id)
        {
            return db.ProductVariants.Find(id);
        }

        public void AddProductVariant(ProductVariant pv)
        {
            db.ProductVariants.Add(pv);
            db.SaveChanges();
        }

        public void EditProductVariant(ProductVariant pv)
        {
            db.Entry(pv).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveProductVariant(ProductVariant pv)
        {
            db.ProductVariants.Remove(pv);
            db.SaveChanges();
        }

        /*=================== ProductVariants ====================*/

        /*=================== ProductCategory ====================*/

        //public IQueryable<ProductCategory> ProductCategories()
        //{
        //    return db.ProductCategories;
        //}

        //public ProductCategory FindProductCategory(int productid, int categoryid)
        //{
        //    return db.ProductCategories.SingleOrDefault(p => p.ProductId == productid && p.CategoryId == categoryid);
        //}

        //public void AddProductCategory(ProductCategory pc)
        //{
        //    db.ProductCategories.Add(pc);
        //    db.SaveChanges();
        //}

        //public void EditProductCategory(ProductCategory pc)
        //{
        //    db.Entry(pc).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        //public void RemoveProductCategory(ProductCategory pc)
        //{
        //    db.ProductCategories.Remove(pc);
        //    db.SaveChanges();
        //}

        //public ProductCategory FindProductCategory(int productcategoryid)
        //{
        //    return db.ProductCategories.Find(productcategoryid);
        //}

        /*=================== ProductCategory ====================*/

        /*=================== OrderProduct ====================*/

        public IQueryable<OrderProduct> OrderProducts()
        {
            return db.OrderProducts;
        }

        public OrderProduct FindOrderProduct(int id)
        {
            return db.OrderProducts.Find(id);
        }

        public OrderProduct FindOrderProduct(int orderid, int productid)
        {
            return db.OrderProducts.SingleOrDefault(p => p.OrderId == orderid && p.ProductId == productid);
        }

        public void AddOrderProduct(OrderProduct op)
        {
            db.OrderProducts.Add(op);
            db.SaveChanges();
        }

        public void EditOrderProduct(OrderProduct op)
        {
            db.Entry(op).State = EntityState.Modified;
            //db.SaveChanges();
        }

        public void RemoveOrderProduct(OrderProduct op)
        {
            db.OrderProducts.Remove(op);
            db.SaveChanges();
        }

        /*=================== OrderProduct ====================*/

        /*=================== BalancerProcessor ====================*/

        public IQueryable<BalancerProcessor> BalancerProcessors()
        {
            return db.BalancerProcessors;
        }

        public BalancerProcessor FindBalancerProcessor(int id)
        {
            return db.BalancerProcessors.Find(id);
        }

        public BalancerProcessor FindBalancerProcessor(int balancerid, int processorid)
        {
            return db.BalancerProcessors.SingleOrDefault(b => b.BalancerId == balancerid && b.ProcessorId == processorid);
        }

        public void AddBalancerProcessor(BalancerProcessor bp)
        {
            db.BalancerProcessors.Add(bp);
            db.SaveChanges();
        }

        public void EditBalancerProcessor(BalancerProcessor bp)
        {
            db.Entry(bp).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveBalancerProcessor(BalancerProcessor bp)
        {
            db.BalancerProcessors.Remove(bp);
            db.SaveChanges();
        }

        /*=================== BalancerProcessor ====================*/

        /*=================== ProductEvent ====================*/

        public IQueryable<ProductEvent> ProductEvents()
        {
            return db.ProductEvents;
        }

        public ProductEvent FindProductEvent(int id)
        {
            return db.ProductEvents.Find(id);
        }

        public ProductEvent FindProductEvent(int productid, int eventid)
        {
            return db.ProductEvents.SingleOrDefault(p => p.ProductId == productid && p.EventId == eventid);
        }

        public void AddProductEvent(ProductEvent pe)
        {
            db.ProductEvents.Add(pe);
            db.SaveChanges();
        }

        public void EditProductEvent(ProductEvent pe)
        {
            db.Entry(pe).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveProductEvent(ProductEvent pe)
        {
            db.ProductEvents.Remove(pe);
            db.SaveChanges();
        }

        /*=================== ProductEvent ====================*/

        public IQueryable<Country> Countries()
        {
            return db.Countries;
        }

        /*=================== CustomField ====================

        public IQueryable<CustomField> CustomFields()
        {
            return db.CustomFields;
        }

        public void AddCustomField(CustomField customfield)
        {
            db.CustomFields.Add(customfield);
            db.SaveChanges();
        }

        public CustomField FindCustomField(int id)
        {
            return db.CustomFields.Find(id);
        }

        public void EditCustomField(CustomField cf)
        {
            db.Entry(cf).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveCustomField(CustomField cf)
        {
            db.CustomFields.Remove(cf);
            db.SaveChanges();
        }

        /*=================== CustomField ====================

        /*=================== CustomFieldValue ====================

        public void AddCustomFieldValue(CustomFieldValue customFieldValue)
        {
            db.CustomFieldValues.Add(customFieldValue);
            db.SaveChanges();
        }

        public IQueryable<CustomFieldValue> CustomFieldValues()
        {
            return db.CustomFieldValues;
        }

        public void RemoveCustomFields(IQueryable<CustomFieldValue> range)
        {
            db.CustomFieldValues.RemoveRange(range);
            db.SaveChanges();
        }

        =================== CustomFieldValue ====================*/

        

        /*=================== VariantExtraField ====================*/

        public void AddVariantExtraField(VariantExtraField variantExtraField)
        {
            db.VariantExtraFields.Add(variantExtraField);
            db.SaveChanges();
        }

        public IQueryable<VariantExtraField> VariantExtraFields()
        {
            return db.VariantExtraFields;
        }

        public void RemoveExtraFields(ProductVariant pv)
        {
            db.VariantExtraFields.RemoveRange(db.VariantExtraFields.Where(x => x.ProductVariantId == pv.ProductVariantId));
            db.SaveChanges();
        }

        /*================================= Decline Salvages =======================================================*/
        //public DeclineSalvage FindDeclineSalvage(int id)
        //{
        //    return db.DeclineSalvages.Find(id);
        //}


        //public void EditDeclineSalvage(DeclineSalvage ds)
        //{
        //    db.Entry(ds).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        //public void AddDeclineSalvage(DeclineSalvage declineSalvage)
        //{
        //    db.DeclineSalvages.Add(declineSalvage);
        //    db.SaveChanges();
        //}


        //public IQueryable<DeclineSalvage> DeclineSalvages()
        //{
        //    return db.DeclineSalvages;
        //}
        /*================================= Decline Salvages =======================================================*/


        public void AddOrderNote(OrderNote ordernote)
        {
            db.OrderNotes.Add(ordernote);
            db.SaveChanges();
        }

        public IQueryable<OrderNote> OrderNotes()
        {
            return db.OrderNotes;
        }


        public IQueryable<UserProfile> UserProfiles()
        {
            return db.UserProfiles;
        }

        public UserProfile FindUserProfile(int id)
        {
            return db.UserProfiles.Find(id);
        }

        public void EditUserProfile(UserProfile user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }


        public void RemoveUserProfile(UserProfile user)
        {
            db.UserProfiles.Remove(user);
            db.SaveChanges();
        }


        public IQueryable<SmtpServer> SmtpServers()
        {
            return db.SmtpServers;
        }


        public void AddSmtpServer(SmtpServer server)
        {
            db.SmtpServers.Add(server);
            db.SaveChanges();
        }

        public void EditSmtpServer(SmtpServer server)
        {
            db.Entry(server).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void RemoveSmtpServer(SmtpServer server)
        {
            db.SmtpServers.Remove(server);
            db.SaveChanges();
        }

        public SmtpServer FindSmtpServer(int id)
        {
            return db.SmtpServers.Find(id);
        }


        public IQueryable<AutoResponderProvider> AutoResponderProviders
        {
            get { return db.AutoResponderProviders; }
        }


        public AutoResponderProvider FindAutoResponderProvider(int id)
        {
            return db.AutoResponderProviders.Find(id);
        }

        public void AddAutoResponderProvider(AutoResponderProvider responder)
        {
            db.AutoResponderProviders.Add(responder);
            db.SaveChanges();
        }

        public void RemoveAutoResponderProvider(AutoResponderProvider responder)
        {
            db.AutoResponderProviders.Remove(responder);
            db.SaveChanges();
        }

        public void EditAutoResponderProvider(AutoResponderProvider responder)
        {
            db.Entry(responder).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void AddLog(KLog klog)
        {
            db.KLogs.Add(klog);
            db.SaveChanges();
        }

        public IQueryable<AuditLog> AuditLogs
        {
            get { return db.AuditLog; }
        }

        public AuditLog FindAuditLog(int id)
        {
            return db.AuditLog.Find(id);
        }


        public DbSet<ShippingCategory> ShippingCategories
        {
            get { return db.ShippingCategories; }
        }


        public void AddShippingCategory(ShippingCategory shippingcategory)
        {
            db.ShippingCategories.Add(shippingcategory);
            db.SaveChanges();
        }

        public ShippingCategory FindShippingCategory(int id)
        {
            return db.ShippingCategories.Find(id);
        }

        public void RemoveShippingCategory(ShippingCategory shippingcategory)
        {
            db.ShippingCategories.Remove(shippingcategory);
            db.SaveChanges();
        }

        public void EditShippingCategory(ShippingCategory shippingcategory)
        {
            db.Entry(shippingcategory).State = EntityState.Modified;
            db.SaveChanges();
        }


        public DbSet<PostBackUrl> PostBackUrls
        {
            get { return db.PostBackUrls; }
        }


        public void RemovePostbackUrls(IEnumerable<PostBackUrl> pburls)
        {
            db.PostBackUrls.RemoveRange(pburls);
            db.SaveChanges();
        }

        public void AddPostbackUrl(PostBackUrl url)
        {
            db.PostBackUrls.Add(url);
            db.SaveChanges();
        }
    }

}