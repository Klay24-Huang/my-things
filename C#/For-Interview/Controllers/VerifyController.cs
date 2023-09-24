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
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;

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
            return View(new VerifyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Search(VerifyViewModel searchViewModel)
        {
            using var dapper = new SqlConnection(_configuration.GetConnectionString("Default"));
            var p = new DynamicParameters();
            var whereConditionList = new List<string>();

            if (!string.IsNullOrWhiteSpace(searchViewModel.Account))
            {
                whereConditionList.Add("u.account LIKE @Account");
                p.Add("@Account", $"%{searchViewModel.Account}%");
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Name))
            {
                whereConditionList.Add("u.name LIKE @Name");
                p.Add("@Name", $"%{searchViewModel.Name}%");
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Email))
            {
                whereConditionList.Add("u.email LIKE @Email");
                p.Add("@Email", $"%{searchViewModel.Email}%");
            }

            if (!string.IsNullOrWhiteSpace(searchViewModel.Organization))
            {

                whereConditionList.Add("o.title LIKE @Org");
                p.Add("@Org", $"%{searchViewModel.Organization}%");
            }

            // 總頁數
            var countSql = @"SELECT 
                COUNT(*) cnt
                FROM users u
                JOIN orgs o ON u.org_id = o.id";

            var whereStr = string.Empty;
            if (p.ParameterNames.Count() > 0)
            {
                whereStr = " WHERE " + string.Join(" AND ", whereConditionList);
                countSql += whereStr;
            }
            var totalCount = await dapper.QueryFirstOrDefaultAsync<int>(countSql, p);
            searchViewModel.TotalPage = totalCount / 10 + 1;

            // table data, search result
            var resultSql = @"SELECT 
                u.*,
                o.title 'organizatoin'
                FROM users u
                JOIN orgs o ON u.org_id = o.id";

            resultSql += whereStr;

            // 分頁 sql
            var pagaSql = @"
                ORDER BY u.id 
                OFFSET @OFFSET ROWS 
                FETCH NEXT 10 ROWS ONLY";
            p.Add("@OFFSET", (searchViewModel.CurrentPage - 1) * 10);

            resultSql += pagaSql;

            var searchResult = await dapper.QueryAsync<UserBase>(resultSql, p);
            searchViewModel.SearchResult = searchResult.ToList();

            return View("index", searchViewModel);
        }

        public async Task<IActionResult> Page(int pageIndex, string name, string email, string account, string organization)
        {
            var model = new VerifyViewModel
            {
                Name = name,
                Email = email,
                Account = account,
                Organization = organization,
                CurrentPage = pageIndex
            };
            return await Search(model);
        }

        public async Task<IActionResult> SendEmail(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            var smtpSetting = _configuration.GetSection("SMTP").Get<SMTP>();

            var client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.Credentials = new NetworkCredential(smtpSetting.SenderEmail, smtpSetting.Password);
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.From = new MailAddress(smtpSetting.SenderEmail, "測試帳號");
            mail.To.Add(user.Email);
            //設定標題
            mail.Subject = "啟用帳號";

            //標題編碼
            mail.SubjectEncoding = Encoding.UTF8;

            //是否使用html當作信件內容主體
            mail.IsBodyHtml = true;

            //信件內容 
            mail.Body = $@"<a href=""https://localhost:7110/activeAccount/{Base64Helper.Encode(user.Account)}"">開通帳號</a>";

            //內容編碼
            mail.BodyEncoding = Encoding.UTF8;
            client.Send(mail);

            return Ok("已寄送認證信");
        }

        // 開通帳號
        [HttpGet("activeAccount/{baseUserAccount}")]
        public async Task<IActionResult> ActiveAccount(string baseUserAccount)
        {
            _logger.LogInformation($"Active {baseUserAccount}");
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
            return File(System.IO.File.OpenRead($"{_environment.ContentRootPath}{applyFile.FilePath}"), "application/octet-stream", Path.GetFileName(applyFile.FilePath));
        }

        public IActionResult ExportExcel(VerifyViewModel searchViewModel)
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

            foreach (var user in searchViewModel.SearchResult)
            {
                var status = user.Status ? "已開通" : "未開通";
                dt.Rows.Add(user.Id, user.Account, user.Name, user.Email, user.Birthday.ToString("yyyy/MM/dd"), user.Organizatoin, status);
            }
            //using ClosedXML.Excel;  
            using XLWorkbook wb = new();
            wb.Worksheets.Add(dt);
            using MemoryStream stream = new();
            wb.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "審核列表.xlsx");
        }
    }
}
