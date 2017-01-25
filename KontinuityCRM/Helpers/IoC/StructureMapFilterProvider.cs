using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Structure Map Filter Provider
    /// </summary>
    public class StructureMapFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IContainer _container;

        public StructureMapFilterProvider(IContainer container)
        {
            _container = container;
        }
        /// <summary>
        /// Get Filters
        /// </summary>
        /// <param name="controllerContext">Controller Context</param>
        /// <param name="actionDescriptor">Action Descriptor</param>
        /// <returns></returns>
        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);

            foreach (var filter in filters)
            {
                // NOTE THE USE OF OBJECTFACTORY
                //ObjectFactory.BuildUp(filter.Instance);
                var injectedfilter = _container.GetInstance(filter.GetType()) as Filter;
                yield return injectedfilter;
            }
        }
    }
}