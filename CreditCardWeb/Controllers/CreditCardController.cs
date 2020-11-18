using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult BindSuccess()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BindSuccess(FormCollection collection)
        {
            string LogPath = "~/Content/CreditCardBindLog";
            string Log = collection.ToString();
            DirectoryInfo di = new DirectoryInfo(Server.MapPath(LogPath));
            if (!di.Exists)
            {
                di.Create();
            }
            string data = Log;
            string filename = string.Format("{0:yyyyMMdd}.txt", System.DateTime.Today);

            System.IO.File.AppendAllText(LogPath + filename, data);
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