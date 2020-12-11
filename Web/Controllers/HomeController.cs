using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.Enum;
using WebAPI.Models.BaseFunc;
using WebCommon;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public ActionResult Index()
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
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            string UserId = (collection["UserId"] == null) ? "" : collection["UserId"].ToString().Replace(" ", "").Replace("'", "").Replace("\"", "");
            string UserPwd = (collection["UserPwd"] == null) ? "" : collection["UserPwd"].ToString().Replace(" ", "").Replace("'", "").Replace("\"", "");
            string ClientIP = GetIp();
            bool flag = true;
            string errCode = "";
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (UserId != "" && UserPwd != "")
            {
                SPInupt_Login SPInput = new SPInupt_Login()
                {
                    Account = UserId,
                    ClientIP = ClientIP,
                    UserPwd = UserPwd
                };
                SPOutput_Login SPOutput = new SPOutput_Login();

                string SPName = new ObjType().GetSPName(ObjType.SPType.Login);
                SQLHelper<SPInupt_Login, SPOutput_Login> sqlHelp = new SQLHelper<SPInupt_Login, SPOutput_Login>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    Session["User"] = SPOutput.UserName;
                    Session["Account"] = UserId;
                    ViewData["Account"] = UserId; 
                    Session["UserGroup"] = SPOutput.UserGroup;
                    ViewData["UserGroup"] = SPOutput.UserGroup;
                    ViewData["IsLogin"] = 1;
                    ViewData["LoginMessage"] = string.Format("{0}您好", SPOutput.UserName);

                }
                else
                {
                    ViewData["Account"] = "";
                    ViewData["IsLogin"] = 0;
                    ViewData["LoginMessage"] = "帳號或密碼錯誤";
                }
            }
            else
            {
                flag = false;
                ViewData["Account"] = "";
                ViewData["IsLogin"] = 0;
                ViewData["LoginMessage"] = "帳號或密碼未輸入";
            }


            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            ViewData.Clear();
            Response.Redirect("../Home/Login");
            return View();
        }
        public ActionResult ChangePWD()
        {
            return View();
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;

        }
    }
}