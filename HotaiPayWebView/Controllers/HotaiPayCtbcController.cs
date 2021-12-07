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

        [HttpGet]
        public ActionResult CreditCardChoose(string AccessToken)
        {
            //List<CreditCardChoose> lstData = new HotaiPayCtbcRepository(connetStr).GetCreditCarLists("C221120413");
            bool flag;
            HotaipayService getlist = new HotaipayService();
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            IFN_QueryDefaultCard input2 = new IFN_QueryDefaultCard();
            HotaiCardInfo card = new HotaiCardInfo();
            string errorcode = "";
            input.IDNO = AccessToken;
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
        public ActionResult InsPersonInfo(TangViewModel inn)
        {
            bool flag;
            HotaipayService addcard = new HotaipayService();
            OFN_HotaiFastAddCard output = new OFN_HotaiFastAddCard();

            OFN_HotaiFastAddCard vm = new OFN_HotaiFastAddCard();

            //IFN_HotaiFastAddCard input = new IFN_HotaiFastAddCard();
            //input.Birthday = BIRTHDATE;
            IFN_HotaiFastAddCard input = new IFN_HotaiFastAddCard()
            {
                Birthday = inn.Birthday,
                IDNO = inn.CTBCIDNO,
                CTBCIDNO= inn.CTBCIDNO,
                RedirectURL= "https://irentcar.com.tw/",
                insUser="",
                LogID= 0,
                PRGName= "InsPersonInfo"
            };
            string errCode = "";
            flag = addcard.DoFastAddCard(input, ref output, ref errCode);
            if (flag)
            {
                vm = output;
            }
            else
            {
                vm.succ = false;
            }

            return Json(vm);
            //return View();
        }
        public ActionResult SuccessBind()
        {
            return View();
        }
    }
}