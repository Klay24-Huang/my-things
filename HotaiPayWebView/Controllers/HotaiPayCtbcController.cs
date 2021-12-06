using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reposotory.Implement;
using System.Configuration;
using HotaiPayWebView.Models;
using HotaiPayWebView.Repository;
using OtherService;
using Domain.Flow.Hotai;
using Domain.TB.Hotai;

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
            //List<CreditCardChoose> lstData = new HotaiPayCtbcRepository(connetStr).GetCreditCarLists("C221120413");
            bool flag;
            HotaipayService getlist = new HotaipayService();
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            IFN_QueryDefaultCard input2 = new IFN_QueryDefaultCard();
            HotaiCardInfo card = new HotaiCardInfo();
            string errorcode="";
            input.IDNO = "A225668592";
            input.PRGName = "CreditCardChoose";
            flag = getlist.DoQueryCardList(input, ref output, ref errorcode);
            //if (flag)
            //{
            //    flag = getlist.DoQueryDefaultCard(input2, ref card, ref errorcode);
            //}

            //CreditCardChoose Data = new CreditCardChoose();
            List<HotaiCardInfo> Data = output.CreditCards;
            //Data.CardNo = "";
            //Data.CardType = "";

            return View(Data);
        }

        public ActionResult ChooseNewCard()
        {
            return View();
        }
        public ActionResult InsPersonInfo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsPersonInfo(string ID,string BIRTHDATE)
        {
            string b = ID;
            return View();
        }
    }
}