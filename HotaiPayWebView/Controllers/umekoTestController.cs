using Domain.Flow.Hotai;
using Domain.TB.Hotai;
using Domain.WebAPI.Input.CTBCPOS;
using HotaiPayWebView.Models;
using Newtonsoft.Json;
using OtherService;
using OtherService.WebAction;
using System;
using System.Net.Http;
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
                IDNO = "F128697972",
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
        public ActionResult AddCard2()
        {
            var vm = new OFN_HotaiAddCard();
            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiAddCard() { IDNO = "C221120413", RedirectURL = "", insUser = "umeko", LogID = 0, PRGName = "Test" };
            var output = new OFN_HotaiAddCard();

            var errCode = "";
            var flag = hotaipayService.DoAddCard(input, ref output, ref errCode);

            ServerPage sendPage = new ServerPage();
            if (flag)
            {
                try { 
                    sendPage.FormId = "AddCard2";
                    sendPage.Target = "_self";
                    sendPage.Url = output.gotoUrl;
                    sendPage.SendText = JsonConvert.SerializeObject(output.postData);
                    sendPage.ServiceMethod = HttpMethod.Post;
                    return new RedirectAndPost(sendPage);
                }
                catch(Exception ex)
                {
                    HttpContext.Response.StatusCode = 403;
                    return Json(new { status = "error", errCode = "", message = ex.Message });
                    }
            }
            else
            {
                HttpContext.Response.StatusCode = 403;
                return Json(new { status = "error", errCode = errCode, message = "" });
            }
            
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
        public ActionResult FastAddCard2(UmekoTestViewModel ivm)
        {
            var vm = new OFN_HotaiFastAddCard();

            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiFastAddCard()
            {
                IDNO = "C221120413"
                ,
                RedirectURL = "",
                CTBCIDNO = ivm.CTBCIDNO,
                Birthday = ivm.Birthday,
                insUser = "umeko",
                LogID = 0,
                PRGName = "Test"
            };
            var output = new OFN_HotaiFastAddCard();
            string errCode = "";
            var flag = hotaipayService.DoFastAddCard(input, ref output, ref errCode);

            ServerPage sendPage = new ServerPage();
            if (flag)
            {

                sendPage.FormId = "FastAddCard2";
                sendPage.Target = "_self";
                sendPage.Url = output.gotoUrl;
                sendPage.SendText = JsonConvert.SerializeObject(output.postData);
                sendPage.ServiceMethod = HttpMethod.Post;

                return new RedirectAndPost(sendPage);
            }
            return Json(errCode);
            
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
                OrderNo = int.Parse(ivm.OrderID),
                //Transaction_no = ivm.OrderID,
                //insUser = "umeko",
                //LogID = 0,
                //PRGName = "Test"
            };
            var output = new OFN_HotaiPaymentAuth();
            string errCode = "";
            var flag = hotaipayService.DoReqPaymentAuth(input, ref output, ref errCode);
            

            return Json(output);
        }
        [HttpPost]
        public JsonResult inquiry(UmekoTestViewModel ivm)
        {
            HotaipayService hotaipayService = new HotaipayService();

            WebAPIInput_InquiryByLidm input = new WebAPIInput_InquiryByLidm();
            string errCode = "";
            input.OrderID = ivm.OrderID;

            var flag = hotaipayService.DoQueryCTBCTransaction(input,out var output, ref errCode);

            return Json(output);
        }

        
    }


}