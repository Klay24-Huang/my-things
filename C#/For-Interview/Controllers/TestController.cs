using ClosedXML.Excel;
using For_Interview.Helper;
using For_Interview.Models;
using For_Interview.Models.DbModels;
using For_Interview.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace For_Interview.Controllers
{
    public class TestController : Controller
    {
        //private readonly ILogger _logger;
        private readonly MyDBContext _dBContext;
        public TestController(MyDBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task<IActionResult> Index()
        {
            var orgs = new List<Org> {
                new Org
                {
                    Title = "A1",
                },
                new Org
                {
                    Title = "B1",
                },
                new Org
                {
                    Title = "C1",
                },

            };
            await _dBContext.AddRangeAsync(orgs);
            await _dBContext.SaveChangesAsync();

            var users = new List<User>();
            var password = Base64Helper.Encode("aaa111aaa11");
            for (var i = 0; i <= 200; i++)
            {
                var user = new User
                {
                    Name = $"使用者{i}",
                    OrgId = orgs[i % 3].Id,
                    Birthday = DateTime.Now,
                    Email = $"test{i}@gmail.com",
                    Account = $"forTest{i}",
                    Password = password,
                    Status = i % 2 == 0,
                };
                users.Add(user);
            }
            await _dBContext.AddRangeAsync(users);
            await _dBContext.SaveChangesAsync();

            var files = new List<ApplyFile>();
            var filePath = @"\UploadFiles\test.jpg";

            for (var i = 0; i <= 200; i++)
            {
                var file = new ApplyFile {
                    UserId = users[i].Id,
                    FilePath = filePath,
                };
                files.Add(file);
            }

            await _dBContext.AddRangeAsync(files);
            await _dBContext.SaveChangesAsync();


            return View(new TestViewModel { MyProperty = 222 });
        }
    }
}
