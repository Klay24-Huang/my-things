using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class AccountManageController : Controller
    {
       /// <summary>
       /// 加盟業者維護
       /// </summary>
       /// <returns></returns>
        public ActionResult FranchiseesMaintain()
        {
            return View();
        }
        /// <summary>
        /// 功能群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncGroupMaintain()
        {
            return View();
        }
        /// <summary>
        /// 功能維護
        /// </summary>
        /// <returns></returns>
        public ActionResult FuncMaintain()
        {
            return View();
        }
        /// <summary>
        /// 使用者群組維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserGroupMaintain()
        {
            return View();
        }
        /// <summary>
        /// 使用者維護
        /// </summary>
        /// <returns></returns>
        public ActionResult UserMaintain()
        {
            return View();
        }
    }
}