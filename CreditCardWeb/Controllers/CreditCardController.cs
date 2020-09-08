using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreditCardWeb.Controllers
{
    public class CreditCardController : Controller
    {
        // GET: CreditCard
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 綁定卡片成功
        /// </summary>
        public ActionResult BindSuccess()
        {
            return View();
        }
        /// <summary>
        /// 綁定卡片失敗
        /// </summary>
        public ActionResult BindFail()
        {
            return View();
        }
    }
}