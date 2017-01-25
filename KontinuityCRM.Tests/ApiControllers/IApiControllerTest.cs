using System.Web.Http;

namespace KontinuityCRM.Tests.ApiControllers
{
    public interface IApiControllerTest<TController> where TController : ApiController 
    {
        TController Controller { get; set; }
    }
}
