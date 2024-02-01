using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class FooController : ApiController
    {
        // GET: Foo

        [HttpGet]
        [Route("api/Foo")]
        public string Index()
        {
            return "Foo";
        }
    }
}