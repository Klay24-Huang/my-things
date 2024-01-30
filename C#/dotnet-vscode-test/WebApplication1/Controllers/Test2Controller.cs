using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class Test2Controller : Controller
    {
        // GET: Test
        public string Index()
        {
            //Task.Run(() => FooAsync()); // Run FooAsync on a separate thread
            Thread.Sleep(30000);
            return "OK";
        }

        private void FooAsync()
        {
            for (int i = 0; i < 3; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Loop count: {i}");
                Thread.Sleep(1000);
            }
        }
    }
}