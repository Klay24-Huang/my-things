using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 客服專區
    /// </summary>
    public class CustomerAreaController : BaseSafeController //20210902唐改繼承BaseSafeController，寫nlog //Controller
    {
        /// <summary>
        /// 合約/中控台
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerDashBoard()
        {
            BaseSafeController himsSafe = new BaseSafeController();
            himsSafe.nnlog(Session["User"], Session["Account"], System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                , "CustomerDashBoard");

            return View();
        }
    }
}