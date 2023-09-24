using For_Interview.Helper;
using For_Interview.Models;
using For_Interview.Models.ConfigModels;
using For_Interview.Models.DbModels;
using For_Interview.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.IO;
using System.Data;
using ClosedXML.Excel;

namespace For_Interview.Controllers
{
    public class VerifyController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<VerifyController> _logger;

        public VerifyController(MyDBContext dBContext, ILogger<VerifyController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _dbContext = dBContext;
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            //var users = new List<User>();
            //for (int i = 0; i < 20; i++)
            //{
            //    users.Add(new User { 
            //        Id = i,
            //        Name = "name",
            //        Account = "aaa",
            //        Email = "aaa@gmail.com"
            //    });
            //}
            //return View(users);
            return View(new VerifyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(VerifyViewModel searchViewModel)
        {
            var condition = searchViewModel.SearchConditoin;
            var p = new List<SqlParameter>();
            var whereConditionList = new List<string>();

            if (string.IsNullOrWhiteSpace(condition.Account))
            {
                whereConditionList.Add("u.account=@account");
                p.Add(new SqlParameter("@account", condition.Account));
            }

            if (string.IsNullOrWhiteSpace(condition.Name))
            {
                whereConditionList.Add("u.name=@name");
                p.Add(new SqlParameter("@name", condition.Name));
            }

            if (string.IsNullOrWhiteSpace(condition.Email))
            {
                whereConditionList.Add("u.email=@email");
                p.Add(new SqlParameter("@email", condition.Email));
            }

            if (string.IsNullOrWhiteSpace(condition.Organizatoin))
            {

                whereConditionList.Add("o.title=@org");
                p.Add(new SqlParameter("@org", condition.Organizatoin));
            }

            // 分頁
            var pagaSql = @"
                ORDER BY u.id 
                OFFSET @OFFSET ROWS 
                FETCH NEXT 10 ROWS ONLY";
            p.Add(new SqlParameter("@OFFSET", (searchViewModel.CurrentPage - 1) * 10));

            var resultSql = @"SELECT 
                u.*,;
                o.title;
                FROM users u;
                JOIN orgs o ON u.org_id = o.id";

            var whereStr = string.Empty;
            if (p.Count > 0)
            {
                whereStr = " where " + string.Join(" and ", whereConditionList);
                resultSql += whereStr;
            }

            resultSql += pagaSql;

            var searchResult = _dbContext.Database.SqlQueryRaw<UserBase>(resultSql, p);
            searchViewModel.SearchResult = searchResult;

            var countSql = @"SELECT 
                COUNT(*)
                FROM users u
                JOIN orgs o ON u.org_id = o.id";

            if (p.Count > 0)
            {
                countSql += whereStr;
            }

            var totalCount = await _dbContext.Database.SqlQueryRaw<int>(countSql, p).FirstOrDefaultAsync();
            searchViewModel.TotalPage = totalCount / 10;

            return View("index", searchViewModel);
        }

        public async Task<IActionResult> PrevOrNext(int pageIndex, VerifyViewModel searchViewModel)
        {
            searchViewModel.CurrentPage = pageIndex;
            return await Search(searchViewModel);
        }

        public async Task SendEmail(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync();

            var smtpSetting = _configuration.Get<SMPT>();
            var client = new SmtpClient
            {
                Host = smtpSetting.Host,
                Port = smtpSetting.Port,
                Credentials = new NetworkCredential(smtpSetting.SenderEmail, smtpSetting.Password),
                EnableSsl = true
            };

            var mail = new MailMessage();
            mail.From = new MailAddress(smtpSetting.SenderEmail, "測試帳號");
            mail.To.Add(user?.Email);
            //設定標題
            mail.Subject = "啟用帳號";

            //標題編碼
            mail.SubjectEncoding = Encoding.UTF8;

            //是否使用html當作信件內容主體
            mail.IsBodyHtml = true;

            //信件內容 
            mail.Body = $@"<a href=""https://localhost:7110/verify/active/{Base64Helper.Encode(user.Account)}"">開通帳號</a>";

            //內容編碼
            mail.BodyEncoding = Encoding.UTF8;
            await client.SendMailAsync(mail);
        }

        // 開通帳號
        [HttpGet("/{baseUserAccount}")]
        public async Task<IActionResult> ActiveAccount(string baseUserAccount)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Account == Base64Helper.Decode(baseUserAccount));
          
            if (user != null)
            {
                user.Status = true;
                user.UpdateAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return Content("帳號開通");
            }

            return Content("無效連結");
        }

        /// <summary>
        /// 下載註冊附檔
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(int userId)
        {
            var applyFile = await _dbContext.ApplyFiles.FirstOrDefaultAsync(x => x.UserId == userId);
            return File(System.IO.File.OpenRead(applyFile.FilePath), "application/octet-stream", Path.GetFileName(applyFile.FilePath));

            //if (System.IO.File.Exists(applyFile.FilePath))
            //{
            //    return File(System.IO.File.OpenRead(applyFile.FilePath), "application/octet-stream", Path.GetFileName(applyFile.FilePath));
            //}
        }

        public async Task<IActionResult> ExportExcel(VerifyViewModel model)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("流水號"),
                new DataColumn("帳號"),
                new DataColumn("姓名"),
                new DataColumn("Email"),
                new DataColumn("生日"),
                new DataColumn("組織"),
                new DataColumn("狀態"),
            });

            foreach (var user in model.SearchResult)
            {
                var status = user.Status ? "已認證" : "未認證";
                dt.Rows.Add(user.Id, user.Account, user.Name, user.Email, user.Birthday, user.Organizatoin, status);
            }
            //using ClosedXML.Excel;  
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "審核列表.xlsx");
                }
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
