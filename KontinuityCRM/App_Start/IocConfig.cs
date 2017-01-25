using ArtisanCode.SimpleAesEncryption;
using AutoMapper;
using KontinuityCRM.Helpers;
using KontinuityCRM.Helpers.IoC;
using KontinuityCRM.Helpers.ScheduledTasks;
using KontinuityCRM.Models;
using Quartz;
using Quartz.Spi;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KontinuityCRM.App_Start
{
    public static class IocConfig
    {
        private static IContainer BuildContainer()
        {
            var _container = new Container(_ =>
            {
                _.For<IContinuityDbContext>().Use<KontinuityDB>();
                _.Policies.SetAllProperties(x => x.OfType<IContinuityDbContext>()); 

                _.For<IUnitOfWork>().Use<UnitOfWork>();
                _.Policies.SetAllProperties(x => x.OfType<IUnitOfWork>()); 
                
                _.For<IWebSecurityWrapper>().Use(new WebSecurityWrapper());
                //_.Policies.SetAllProperties(x => x.OfType<IWebSecurityWrapper>()); 

                //_.For<System.Web.ModelBinding.ModelValidatorProvider>().Use<KontinuityCRM.Helpers.IoC.CustomModelValidatorProvider>();

                _.For<IMappingEngine>().Use(Mapper.Engine);
                _.For<INLogger>().Use<NInLogger>();
                _.For<IMessageEncryptor>().Use(new RijndaelMessageEncryptor());
                _.For<IMessageDecryptor>().Use(new RijndaelMessageDecryptor());
                _.For<ISchedulerFactory>().Use<StructureMapSchedulerFactory>();
                _.For<IScheduler>().Use(o => o.GetInstance<ISchedulerFactory>().GetScheduler());
                _.For<IQuartzScheduler>().Use<JobScheduler>();

                _.For<HttpConfiguration>().Use(GlobalConfiguration.Configuration); // for the help page controller

                _.Scan(scan =>
                {
                    scan.WithDefaultConventions();
                    AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetName().Name.StartsWith("KontinuityCRM"))
                    .ToList()
                    .ForEach(a => scan.Assembly(a));
                });

            });

            
            _container.Name = "KContainer";
            
            return _container;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IContainer Container { get; private set; }

        public static IContainer RegisterDependencyResolver(HttpConfiguration config)
        {
            var container = BuildContainer();

            // set dependency for WebApi
            config.DependencyResolver = new StructureMapDependencyResolver(container);

            // set dependecy for MVC
            DependencyResolver.SetResolver(new StructureMapDependency(container));

            Container = container;

            return container;
            
        }
    }
    
}