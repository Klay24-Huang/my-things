using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 優惠管理
    /// </summary>
    public class MessageController : BaseSafeController //20210902唐改繼承BaseSafeController，寫nlog //Controller
    {
        /// <summary>
        /// 推播訊息
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "MessageController_Index");

            return View();
        }
    }
}