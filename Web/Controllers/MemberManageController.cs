using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 會員管理
    /// </summary>
    public class MemberManageController : Controller
    {
       /// <summary>
       /// 會員審核
       /// </summary>
       /// <returns></returns>
        public ActionResult Audit()
        {
            return View();
        }
    }
}