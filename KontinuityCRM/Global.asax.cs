using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using KontinuityCRM.Helpers.ScheduledTasks;
using WebMatrix.WebData;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.App_Start;
using KontinuityCRM.Helpers.IoC;
using System.Net;

namespace KontinuityCRM
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application Bootstrapped, starting point which includess mappings, bindings 
        /// </summary>
        protected void Application_Start()
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: false);

            StructureMap.IContainer container = IocConfig.RegisterDependencyResolver(GlobalConfiguration.Configuration);

            // works only for MVC I don't know how inject into the web api model validation
            /* Allows setter injection in validation attributes*/
            DataAnnotationsModelValidatorProvider.RegisterDefaultAdapterFactory(
              (metadata, context, attribute) => new KontinuityCRM.Helpers.IoC.StructureMapDataAnnotationsModelValidator(metadata, context, attribute, container));

            //var p = ModelValidatorProviders.Providers;

            //foreach (var item in p)
            //{
            //    var c = item;
            //}

            //ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.SingleOrDefault(m => m.GetType().));

            // OJO this works!! do not delete !!!
            //ModelValidatorProviders.Providers.Clear();
            //ModelValidatorProviders.Providers.Add(new CustomModelProviderMVC(container));
           
            GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Validation.ModelValidatorProvider), new CustomModelValidatorProvider(container));
            
            SecurityCheck.FillRoles();
            DynamicPermissions.GeneratePermissions();
            AreaRegistration.RegisterAllAreas();
            
            //if (KontinuityCRM.Helpers.KontinuityCRMConfiguration.CreateDynamicAssembly)
            //{
            //    KontinuityCRM.Helpers.DynamicAssembly.Create();
            //}

            // to set a custom filter provider
            //IFilterProvider filterProvider = FilterProviders.Providers.Single(p => p is FilterAttributeFilterProvider); 
            //FilterProviders.Providers.Remove(filterProvider); 
            //var unityFilterAttributeFilterProvider = new UnityFilterAttributeFilterProvider(this.unityBootstrapper.ContainerInstance); 
            //FilterProviders.Providers.Add(unityFilterAttributeFilterProvider);

            WebApiConfig.Register(GlobalConfiguration.Configuration, container);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, container);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            /* Allows setter injection in IValidatableObject viewModels*/
            //DataAnnotationsModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory(
            //    (metadata, context) => new StructureMapValidatableObjectAdapterFactory(metadata, context, container));

            DtoMapperConfig.CreateMaps();

            ModelBinders.Binders.Add(typeof(KontinuityCRM.Models.AutoResponders.AutoResponder), new CustomModelBinder());
            ModelBinders.Binders.Add(typeof(KontinuityCRM.Models.Fulfillments.Fulfillment), new CustomModelBinder());
            ModelBinders.Binders.Add(typeof(KontinuityCRM.Models.Gateways.GatewayModel), new CustomModelBinder());
            
            ModelBinders.Binders.Add(typeof(UrlOrderAction), new UrlOrderActionModelBinder());

#if DEBUG
            // Warning : In debug don't run scheduler job. If you run then make sure you didn't connected to live db.
#else
                   IQuartzScheduler scheduler = container.GetInstance<IQuartzScheduler>();
                            scheduler.Start();
#endif

        }

        protected void Application_BeginRequest()
        {
            if (Request.HttpMethod == "OPTIONS")
            {
                Response.StatusCode = (int)HttpStatusCode.OK;
                //Response.AppendHeader("Access-Control-Allow-Origin", "http://garciniabiofit.com");
                Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept,APIKey");
                Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                Response.AppendHeader("Access-Control-Allow-Credentials", "true");
                Response.End();
            }
        }
    }
   
}