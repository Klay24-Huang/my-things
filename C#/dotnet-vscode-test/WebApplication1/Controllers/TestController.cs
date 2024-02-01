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
    public class TestController : Controller
    {
        // GET: Test
        // https://localhost:44347/Test/Index?path=somePathValue
        public string Index(string path)
        {
            Task.Run(() => FooAsync()); // Run FooAsync on a separate thread
            Thread.Sleep(10000);
            System.Diagnostics.Debug.WriteLine($"return {path}");
            return path;
        }

        private void FooAsync()
        {
            for (int i = 0; i < 20; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Loop count: {i}");
                Thread.Sleep(1000);
            }
        }
    }
}