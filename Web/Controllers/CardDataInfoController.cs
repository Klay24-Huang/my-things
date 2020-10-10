using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 卡片管理
    /// </summary>
    public class CardDataInfoController : Controller
    {
        /// <summary>
        /// 萬用卡管理
        /// </summary>
        /// <returns></returns>
        public ActionResult MasterCardSetting()
        {
            return View();
        }
        /// <summary>
        /// 發送卡號設定
        /// </summary>
        /// <returns></returns>
        public ActionResult SentCardSetting()
        {
            return View();
        }
    }
}