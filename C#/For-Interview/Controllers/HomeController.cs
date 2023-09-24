using For_Interview.Helper;
using For_Interview.Models;
using For_Interview.Models.DbModels;
using For_Interview.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace For_Interview.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MyDBContext dBContext, ILogger<HomeController> logger)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", login);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Account == login.Account && u.Password == Base64Helper.Encode(login.Password));

            var log = new Syslog
            {
                Account = login.Account,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            };

            if (user == null)
            {
                TempData["LoginResult"] = "帳號或密碼錯誤";
            }
            else if (user != null && !user.Status)
            {
                TempData["LoginResult"] = "帳號尚未啟用";
            }
            else
            {
                TempData["LoginResult"] = "登入成功";
                log.LoginAt = DateTime.Now;
            }
            await _dbContext.Syslogs.AddAsync(log);
            await _dbContext.SaveChangesAsync();

            return View("Index", login);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}