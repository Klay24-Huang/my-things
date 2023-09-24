using For_Interview.Helper;
using For_Interview.Models;
using For_Interview.Models.DbModels;
using For_Interview.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace For_Interview.Controllers
{
    public class RegisterController : Controller
    {
        private readonly MyDBContext _dBContext;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(MyDBContext dBContext, ILogger<RegisterController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _dBContext = dBContext;
            _environment = hostingEnvironment;
        }


        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                { 
                    return View("Index", model);
                }

                var orgTitle = model.Organizatoin.Trim();
                var organization = await _dBContext.Orgs.FirstOrDefaultAsync(x => x.Title == orgTitle);

                using var trasaction = await _dBContext.Database.BeginTransactionAsync();
                var newUser = new User
                {
                    Name = model.Name,
                    Birthday = model.Birthday,
                    Email = model.Email,
                    Account = model.Account,
                    Password = Base64Helper.Encode(model.Password),
                    Status = false,
                };

                if (organization != null)
                {
                    newUser.OrgId = organization.Id;
                }
                else
                {
                    var newOrg = new Org { Title = orgTitle };
                    await _dBContext.Orgs.AddAsync(newOrg);
                    await _dBContext.SaveChangesAsync();
                    newUser.OrgId = newOrg.Id;
                }

                await _dBContext.Users.AddAsync(newUser);
                await _dBContext.SaveChangesAsync();

                // copy file to server                
                var file = model.File;
                
                if (file != null && file.Length > 0)
                {
                    var path = $@"{_environment.ContentRootPath}\UploadFiles\{file.FileName}";
                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                }

                var newApplyFile = new ApplyFile
                {
                    UserId = newUser.Id,
                    FilePath = $@"\UploadFiles\{file.FileName}",
                };

                await _dBContext.ApplyFiles.AddAsync(newApplyFile);
                await _dBContext.SaveChangesAsync();
                await trasaction.CommitAsync();
                TempData["RegisterResult"] = "註冊成功";
            }
            catch (Exception e)
            {
                TempData["RegisterResult"] = "註冊失敗";
                _logger.LogError(null, e, null);
                throw;
            }
            return View("Index", model);
        }
    }
}
