using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using KontinuityCRM.Helpers;
using System.Web.Http.Cors;

namespace KontinuityCRM
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, StructureMap.IContainer container)
        {
            //config.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });

            // this works !!!
            //config.MapHttpAttributeRoutes();
            // New code
          //  config.EnableCors();
           // var cors = new EnableCorsAttribute("http://garciniabiofit.com","*","*");
            config.EnableCors();

            //config.UseCors(builder =>
            //    builder.WithOrigins("http://some.origin.com"));

            config.Filters.Add((container.GetInstance<KontinuityCRM.Filters.CustomHandleApiErrorAttribute>()));

            //config.Routes.MapHttpRoute("DefaultApi1", "api/{controller}/{id}", new { id = RouteParameter.Optional, action = "Get" }, new { id = @"\d+", httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });

            //config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }); // , new { action != "post" }

            config.Routes.MapHttpRoute("find", "api/orders/find",  // {ip}
               new { controller = "orders", action = "find" }, // , ip = RouteParameter.Optional
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { action = "Get", id = RouteParameter.Optional }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
           
            
          
            
            config.Routes.MapHttpRoute("rebill", "api/orders/rebill/{id}", 
                new { controller = "orders", action = "rebill" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //config.Routes.MapHttpRoute("tmp", "api/orders/tmp/{id}",
            //   new { controller = "orders", action = "tmp" },
            //   new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("process", "api/orders/process/{id}", new { controller = "orders", action = "process" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("upsell", "api/orders/upsell/{id}", new { controller = "orders", action = "upsell" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("partialtoOrder", "api/partials/order/{id}", new { controller = "partials", action = "order" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            
            config.Routes.MapHttpRoute("DefaultApiPut", "api/{controller}", new { action = "Put" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("DefaultApiDelete", "api/{controller}", new { action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

           
            //config.Routes.MapHttpRoute(
            //    name: "RpcApi",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            //var repository = config.DependencyResolver.GetService(typeof(EFKontinuityCRMRepo)) as IKontinuityCRMRepo;
            var uow = config.DependencyResolver.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            var logger = config.DependencyResolver.GetService(typeof(INLogger)) as INLogger;

            //We have registered our handlers as global handlers, so they will be plugged into the pipeline for all the routes.
            //It is possible to configure the handlers specifically for a route, as per-route message handlers. 
            // Page 216 Practical ASP.Net web api
            //config.MessageHandlers.Add(new AuthenticationHandler());
            config.MessageHandlers.Add(new APIKeyAuthenticationHandle(uow, logger));

            var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None; // remove the $id
            config.Formatters.Remove(config.Formatters.XmlFormatter);

        }
    }
}
