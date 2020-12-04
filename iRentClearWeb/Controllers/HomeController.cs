using Domain.TB;
using Domain.TB.Maintain;
using iRentClearWeb.Models;
//using iRentClearWeb.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;
//using WebModel;

namespace iRentClearWeb.Controllers
{
    public class HomeController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 首頁，改為列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // var ticket = (FormsIdentity)User.Identity;
            if (User == null)
            {
                RedirectToAction("Login");
            }
            return View();
        }
        /// <summary>
        /// 地圖頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Map()
        {
            if (User == null)
            {
                RedirectToAction("Login");
            }
            return View();
        }
        /// <summary>
        /// 設定所屬據點及下載
        /// </summary>
        /// <returns></returns>
        public ActionResult SettingStation()
        {
            bool flag = true;
            if (User == null)
            {
                RedirectToAction("Login");
            }
            else
            {
                string Account = "";
                string UserName = "";
                if (Session["Account"] == null || string.IsNullOrWhiteSpace(Session["Account"].ToString()))
                {
                    Response.Redirect("~/Home/Login");
                }
                else
                {
                    Account = Session["Account"].ToString();
                    UserName = Session["UserName"].ToString();
                }
            }
            if (flag)
            {

            }
            return View();
        }
        public ActionResult Booking(String CarNo, DateTime? BookingStart, DateTime? BookingEnd)
        {
            ViewData["CarNo"] = CarNo;
            ViewData["BookingStart"] = BookingStart;
            ViewData["BookingEnd"] = BookingEnd;
            return View();
        }
        public ActionResult BookingList(String CarNo)
        {
            return View();
        }
        public ActionResult BookingStart()
        {
            return View();
        }
        public ActionResult BookingEnd()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ReportDownload(string[] StationID)
        {
            CarRepository repository = new CarRepository(connetStr);
            string tmpStationID = "";
            if (StationID != null)
            {
                if (StationID.Length > 1)
                {
                    tmpStationID = StationID[0];
                    int len = StationID.Length;
                    for (int i = 1; i < len; i++)
                    {
                        tmpStationID += string.Format(";{0}", StationID[i]);
                    }
                }
                else if (StationID.Length == 1)
                {
                    tmpStationID = StationID[0];
                }
            }
            List<CarCleanDataNew> lstCar = repository.GetCleanCarData(tmpStationID);
            List<CarCleanDataNew> lstMoto = repository.GetCleanMotoData(tmpStationID);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("搜尋結果");
            string[] headerField = { "管轄門市", "iRent據點", "車號", "未清潔天數", "清潔後已出租次數" };
            int headerFieldLen = headerField.Length;

            IRow header = sheet.CreateRow(0);
            for (int j = 0; j < headerFieldLen; j++)
            {
                header.CreateCell(j).SetCellValue(headerField[j]);
                sheet.AutoSizeColumn(j);
            }
            int lstCarLen = (lstCar == null) ? 0 : lstCar.Count;
            int lstMotoLen = (lstMoto == null) ? 0 : lstMoto.Count;
            DateTime NowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            int lastCount = 0;
            if (lstCarLen > 0)
            {
                for (int k = 0; k < lstCarLen; k++)
                {
                    IRow content = sheet.CreateRow(k + 1);
                    content.CreateCell(0).SetCellValue(lstCar[k].ManageStationID);   //管轄門市
                    content.CreateCell(1).SetCellValue(lstCar[k].NowStationName);   //iRent據點
                    content.CreateCell(2).SetCellValue(lstCar[k].CarNo);            //車牌號碼
                    DateTime LastClean = new DateTime(lstCar[k].LastCleanTime.Year, lstCar[k].LastCleanTime.Month, lstCar[k].LastCleanTime.Day, 0, 0, 0);
                    content.CreateCell(3).SetCellValue(NowDate.Subtract(LastClean).TotalDays);   //未清潔天數
                    content.CreateCell(4).SetCellValue(lstCar[k].NewRentCount);   //清潔後已出租次數
                    lastCount = k + 1;
                }
            }
            if (lstMotoLen > 0)
            {
                for (int k = 0; k < lstMotoLen; k++)
                {
                    IRow content = sheet.CreateRow(lastCount + k);
                    content.CreateCell(0).SetCellValue(lstMoto[k].ManageStationID);   //管轄門市
                    content.CreateCell(1).SetCellValue(lstMoto[k].NowStationName);   //iRent據點
                    content.CreateCell(2).SetCellValue(lstMoto[k].CarNo);            //車牌號碼
                    DateTime LastClean = new DateTime(lstMoto[k].LastCleanTime.Year, lstMoto[k].LastCleanTime.Month, lstMoto[k].LastCleanTime.Day, 0, 0, 0);
                    content.CreateCell(3).SetCellValue(NowDate.Subtract(LastClean).TotalDays);   //未清潔天數
                    content.CreateCell(4).SetCellValue(lstMoto[k].NewRentCount);   //清潔後已出租次數
                    //lastCount = k + 1;
                }
            }
            for (int l = 0; l < headerFieldLen; l++)
            {
                sheet.AutoSizeColumn(l);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
           // workbook.Close();
            return base.File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "報表_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");

        }
        /// <summary>
        /// 呈現後台使用者登入頁
        /// </summary>
        /// <param name="ReturnUrl">使用者原本Request的Url</param>
        /// <returns></returns>
        [AllowAnonymous]

        public ActionResult Login(string ReturnUrl)
        {
            //ReturnUrl字串是使用者在未登入情況下要求的的Url
            if ("" == ReturnUrl) { ReturnUrl = "Index"; }
            UserAccount uAccount = new UserAccount() { ReturnUrl = ReturnUrl };
            return View(uAccount);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserAccount uAccount)
        {

            //沒通過Model驗證(必填欄位沒填，DB無此帳密)
            if (!ModelState.IsValid)
            {
                return View(uAccount);
            }

            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
            //"iRentClear"+uAccount.UserName+"⊙"+uAccount.Account,
            //DateTime.Now,
            //DateTime.Now.AddMinutes(30),
            //false,
            //uAccount.AUTHGPNO,
            //FormsAuthentication.FormsCookiePath);

            //// Encrypt the ticket.
            //string encTicket = FormsAuthentication.Encrypt(ticket);

            //// Create the cookie.
            //Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            //都成功...
            //進行表單登入 ※之後User.Identity.Name的值就是vm.Account帳號的值
            //導向預設Url(Web.config裡的defaultUrl定義)或使用者原先Request的Url
            //FormsAuthentication.RedirectFromLoginPage(uAccount.UserName, false);
            //剛剛已導向，此行不會執行到
            //return Redirect(FormsAuthentication.GetRedirectUrl(uAccount.UserName, false));
            if (uAccount.AUTHGPNO == "10")
            {
                Session["UserName"] = uAccount.UserName;
                Session["Account"] = uAccount.Account;
                Session["AUTHGPNO"] = uAccount.AUTHGPNO;
                uAccount.ErrorMessage = "";
                return Redirect("~/Home/Index");
            }
            else
            {
                uAccount.ReturnUrl = "Index";
                uAccount.ErrorMessage = "沒有使用整備網站的權限，請洽企劃人員!";
                return View(uAccount);
            }
        }
        /// <summary>
        /// 後台使用者登出動作
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Logout()
        {
            //清除Session中的資料
            Session.Abandon();
            //登出表單驗證
            FormsAuthentication.SignOut();
            //導至登入頁
            return RedirectToAction("Login", "Home");
        }
    }
}