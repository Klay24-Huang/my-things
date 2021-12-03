using Domain.Flow.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.CTBCPOS;
using Domain.WebAPI.Input.Hotai.Payment;
using Domain.WebAPI.output.Hotai.Payment;
using HotaiPayWebView.Models;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotaiPayWebView.Controllers
{
    public class umekoTestController : Controller
    {
        // GET: umekoTest
        public ActionResult Index()
        {
            var vm = new UmekoTestViewModel();



            var vm1 = new HotaiCardInfo();
            

            vm1.BankDesc = "aaaaa\nbbbbb\nccccc\nddddddd\n";
            vm.hotaiCardInfo = vm1;


            return View(vm);
        }

        public ActionResult GetList()
        {
            var vm = new UmekoTestViewModel();
            HotaipayService hotaipayService = new HotaipayService();

            IFN_QueryCardList ifnInput = new IFN_QueryCardList
            {
                LogID = 0,
                IDNO = "C221120413",
                PRGName = "testPage",
                insUser = "umeko"
            };

            OFN_HotaiCreditCardList ofnOutput = new OFN_HotaiCreditCardList();

            string errorCode = "";
            var flag = hotaipayService.DoQueryCardList(ifnInput, ref ofnOutput, ref errorCode);

            if(flag)
            {
                vm.oFN_HotaiCreditCardList = ofnOutput;
            }

            return View(vm);

        }
        [HttpPost]
        public JsonResult AddCard()
        {
            var vm = new OFN_HotaiAddCard();
            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiAddCard() { IDNO = "C221120413", RedirectURL = "", insUser = "umeko", LogID = 0, PRGName = "Test" };
            var output = new OFN_HotaiAddCard();

            var errCode = "";
            var flag = hotaipayService.DoAddCard(input, ref output, ref errCode);

            if(flag)
            {
                vm = output;
            }
            else
            {
                vm.succ = false;
            }

            return Json(vm);
        }
        [HttpPost]
        public JsonResult FastAddCard(UmekoTestViewModel ivm)
        {
            var vm = new OFN_HotaiFastAddCard();

            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiFastAddCard() {
                IDNO = "C221120413"
                , RedirectURL = "",
                CTBCIDNO = ivm.CTBCIDNO,
                Birthday = ivm.Birthday,
                insUser = "umeko", LogID = 0, PRGName = "Test" };
            var output = new OFN_HotaiFastAddCard();
            string errCode = "";
            var flag =  hotaipayService.DoFastAddCard(input, ref output, ref errCode);
            if (flag)
            {
                vm = output;
            }
            else
            {
                vm.succ = false;
            }

            return Json(vm);
        }


        [HttpPost]
        public JsonResult CrditAuth(UmekoTestViewModel ivm)
        {
            //var vm = new OFN_HotaiFastAddCard();

            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiPaymentAuth()
            {   CardToken = ivm.CardToken.ToString(),
                Amount = 50,
                AuthType = 1,
                AutoClose = 0,
                IDNO = "C221120413",
                OrderNo = 999999,
                Transaction_no = ivm.OrderID
            };
            //var output = new OFN_HotaiFastAddCard();
            string errCode = "";
            var flag = hotaipayService.DoReqPaymentAuth(input,ref errCode);
            

            return Json(flag);
        }
        [HttpPost]
        public JsonResult inquiry(UmekoTestViewModel ivm)
        {
            HotaipayService hotaipayService = new HotaipayService();

            WebQPIInput_InquiryByLidm input = new WebQPIInput_InquiryByLidm();
            string errCode = "";
            var flag = hotaipayService.DoQueryCTBCTransaction(input,out var output, ref errCode);

            return Json(output);
        }

        
    }


}