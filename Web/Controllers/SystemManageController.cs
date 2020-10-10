using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 系統管理
    /// </summary>
    public class SystemManageController : Controller
    {
        /// <summary>
        /// 訊息記錄查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult MessageQuery()
        {
            return View();
        }
        /// <summary>
        /// 車輛管理時間查詢
        /// </summary>
        /// <returns></returns>

        public ActionResult CarTimelineQuery()
        {
            return View();
        }
    }
}