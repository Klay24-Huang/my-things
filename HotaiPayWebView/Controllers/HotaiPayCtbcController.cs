using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reposotory.Implement;
using System.Configuration;
using HotaiPayWebView.Models;
using HotaiPayWebView.Repository;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayCtbcController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRentT"].ConnectionString;
        private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);

        // GET: HotaiPayCtbc
        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        public ActionResult CreditCardChoose()
        {
            List<CreditCardChoose> lstData = new HotaiPayCtbcRepository(connetStr).GetCreditCarLists("C221120413");
            return View(lstData);
        }

    }
}