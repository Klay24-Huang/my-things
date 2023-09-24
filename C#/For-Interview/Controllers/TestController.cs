using ClosedXML.Excel;
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
        public TestController(MyDBContext dBContext) { 
            _dBContext = dBContext;
        }
        public IActionResult Index()
        {
            return View(new TestViewModel { MyProperty = 222});
        }

        public class Employee
        {
            public int EmpID { get; set; }
            public string EmpName { get; set; } = string.Empty;
        }

        //public async Task TestParam(int myProperty, TestViewModel model)
        //{
            //Console.WriteLine(model.MyProperty);
            //    Console.WriteLine(myProperty);


            //    var testdata = new List<Employee>()
            //{
            //    new Employee(){ EmpID=101, EmpName="Johnny"},
            //    new Employee(){ EmpID=102, EmpName="Tom"},
            //    new Employee(){ EmpID=103, EmpName="Jack"},
            //    new Employee(){ EmpID=104, EmpName="Vivian"},
            //    new Employee(){ EmpID=105, EmpName="Edward"},
            //};
            //    //using System.Data;  
            //    DataTable dt = new DataTable("Grid");
            //    dt.Columns.AddRange(new DataColumn[2] { new DataColumn("EmpID"),
            //                            new DataColumn("EmpName") });

            //    foreach (var emp in testdata)
            //    {
            //        dt.Rows.Add(emp.EmpID, emp.EmpName);
            //    }
            //    //using ClosedXML.Excel;  
            //    using (XLWorkbook wb = new XLWorkbook())
            //    {
            //        wb.Worksheets.Add(dt);
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            wb.SaveAs(stream);
            //            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
            //        }
            //    }


            //var client = new SmtpClient();
            //client.Host = "smtp.gmail.com";
            //client.Port = 587;
            //client.Credentials = new NetworkCredential("wkus963@gmail.com", "rtle uysi prrg ecpq");
            //client.EnableSsl = true;

            //var mail = new MailMessage();
            //mail.From = new MailAddress("wkus963@gamil.com", "測試帳號");
            //mail.To.Add("sean200365@hotmail.com");
            ////設定標題
            //mail.Subject = "啟用帳號";

            ////標題編碼
            //mail.SubjectEncoding = Encoding.UTF8;

            ////是否使用html當作信件內容主體
            //mail.IsBodyHtml = true;

            ////信件內容 
            //mail.Body = "adfdsafdasgewqg";

            ////內容編碼
            //mail.BodyEncoding = Encoding.UTF8;
            //client.Send(mail);
            //return Ok();


            //var log = await _dBContext.Syslogs.FirstOrDefaultAsync();
            //Console.WriteLine(log);
            //_logger.LogInformation("some info");
            //var log = new Syslog
            //{
            //    Account = "asdfaaa",
            //    Ipaddress = "127.0.10.10",
            //};

            //_dBContext.Syslogs.Add(log);
            //await _dBContext.SaveChangesAsync();

            //return Ok();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
