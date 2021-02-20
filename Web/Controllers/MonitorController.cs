using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using WebAPI.Models.BaseFunc;//20210218唐加
//using WebCommon;//20210218唐加
//using Domain.SP.BE.Input;//20210218唐加
//using Domain.SP.BE.Output;//20210218唐加
//using Web.Models.Enum;//20210218唐加

namespace Web.Controllers
{
    /// <summary>
    /// 地圖監控
    /// </summary>
    public class MonitorController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 地圖監控
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? Mode, string OrderNum, DateTime? start, DateTime? end, string objCar)
        {
            Int16 sMode = 2;
            List<BE_EvTimeLine> lstEv = null;
            if (Mode != null)
            {
                sMode = Convert.ToInt16(Mode);
                ViewData["Mode"] = sMode;
            }
            if (sMode < 2)
            {
                Int64 tmpOrder = (string.IsNullOrEmpty(OrderNum)) ? 0 : Convert.ToInt64(OrderNum.Replace("H", ""));
                string SD = (start == null) ? "" : Convert.ToDateTime(start).ToString("yyyy-MM-dd HH:mm:ss");
                string ED = (end == null) ? "" : Convert.ToDateTime(end).ToString("yyyy-MM-dd HH:mm:ss");
                string CarNo = (string.IsNullOrEmpty(objCar)) ? "" : objCar;
                EventHandleRepository _respository = new EventHandleRepository(connetStr);

                switch (sMode)
                {
                    case 0:
                        if (tmpOrder > 0)
                        {
                            ViewData["OrderNum"] = tmpOrder;

                            lstEv = _respository.GetMapDataByTimeLine(tmpOrder);
                        }
                        break;
                    case 1:
                        ViewData["CarNo"] = CarNo;
                        ViewData["SD"] = SD;
                        ViewData["ED"] = ED;
                        lstEv = _respository.GetMapDataByTimeLine(objCar, SD, ED);
                        break;
                }
            }
            return View(lstEv);
        }


        /*
        //20210218唐加
        public ActionResult Login ()
        {
            return View();
        }
        //20210218唐加
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
        //20210218唐加
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;

        }
        */
    }
}