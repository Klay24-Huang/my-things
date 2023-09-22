using For_Interview.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace For_Interview.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IWebHostEnvironment _environment;

        public RegisterController(ILogger<RegisterController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _environment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterViewModel form)
        {
            _logger.LogTrace(form.Email);

            if (form.File != null)
            {
                //string _FileName = Path.GetFileName(form.File.FileName);
                var a = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Console.WriteLine(a);
                Console.WriteLine(a + "\\" + form.File);

                using (var stream = System.IO.File.Create(a +"\\"+ form.File.FileName))
                {
                    await form.File.CopyToAsync(stream);
                }
            }
            
            TempData["Test"] = "foooo";
            return View(form);
        }

        public IActionResult Download()
        {
            //var path = @"C:\code\my-things\C#\For-Interview\UploadFiles\";
            //var a = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //FileStream stream = new FileStream(a+"\\"+ "2023 Uark C# 題目.pdf", FileMode.Open, FileAccess.Read, FileShare.Read);
            //return File(stream, "application/octet-stream", "abc.pdf");

            // get root path
            Console.WriteLine(this._environment.ContentRootPath);
            return View();
        }
    }
}
