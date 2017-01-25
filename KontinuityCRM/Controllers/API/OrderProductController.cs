using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers.API
{
    public class OrderProductController : ApiController
    {
        private IKontinuityCRMRepo repo;

        public OrderProductController()
        {
            repo = new EFKontinuityCRMRepo();
        }

        //// POST api/order
        //public HttpResponseMessage Post(OrderProduct op)
        //{
        //    return null;
        //}

    }
}
