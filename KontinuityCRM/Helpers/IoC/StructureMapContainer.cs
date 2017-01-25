using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Resolver for the Web .API
    /// </summary>
    public class StructureMapDependencyResolver : StructureMapDependencyScope, IDependencyResolver
    {
        private readonly IContainer container = null;
        public StructureMapDependencyResolver(IContainer container)
            : base(container)
        {
            this.container = container;
        }
        /// <summary>
        /// Begin scope
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            return new StructureMapDependencyScope(container.GetNestedContainer());
        }
    }

    /// <summary>
    /// Resolver for MVC
    /// </summary>
    public class StructureMapDependency : ServiceLocatorImplBase, IDependencyScope
    {
        protected readonly IContainer Container;

        public StructureMapDependency(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.Container = container;
        }

        public void Dispose()
        {
            this.Container.Dispose();
        }
        /// <summary>
        /// Get Service
        /// </summary>
        /// <param name="serviceType">type</param>
        /// <returns></returns>
        public override object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                return null;
            }

            try
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                           ? this.Container.TryGetInstance(serviceType)
                           : this.Container.GetInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Get services
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.Container.GetAllInstances(serviceType).Cast<object>();
        }
        /// <summary>
        /// Get All Instances 
        /// </summary>
        /// <param name="serviceType">type</param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.Container.GetAllInstances(serviceType).Cast<object>();
        }
        /// <summary>
        /// Get Instance
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return serviceType.IsAbstract || serviceType.IsInterface
                           ? this.Container.TryGetInstance(serviceType)
                           : this.Container.GetInstance(serviceType);
            }

            return this.Container.GetInstance(serviceType, key);
        }
    }
}