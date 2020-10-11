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
    public class CustomerAreaController : Controller
    {
        /// <summary>
        /// 合約/中控台
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerDashBoard()
        {
            return View();
        }
    }
}