using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BusinessManageController : BaseSafeController //20210907唐改繼承BaseSafeController，寫nlog //Controller
    {
        /// <summary>
        /// 儀表板
        /// </summary>
        /// <returns></returns>
        public ActionResult DashBoard()
        {
            //20210907唐加，記錄每支功能使用
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "DashBoard_View");

            return View();
        }
    }
}