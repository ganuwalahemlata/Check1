using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM
{
    public class FilterConfig
    {
        //[StructureMap.Attributes.SetterProperty]
        //public static IUnitOfWork uowxx { get; set; }

        //[StructureMap.Attributes.SetterProperty]
        //public static IUnitOfWork uow { get; set; }
        //[StructureMap.Attributes.SetterProperty]
        //public static IWebSecurityWrapper wsw { get; set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters, StructureMap.IContainer container)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(container.GetInstance<CustomHandleErrorAttribute>());
            //filters.Add(new CustomHandleErrorAttribute(uow, wsw));
        }
    }
}