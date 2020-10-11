using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BusinessManageController : Controller
    {
        /// <summary>
        /// 儀表板
        /// </summary>
        /// <returns></returns>
        public ActionResult DashBoard()
        {
            return View();
        }
    }
}