using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace KontinuityCRM.Controllers.API
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class helperController : ApiController
    {

        public int Get()
        {
            return 1;
        }
    }
}
