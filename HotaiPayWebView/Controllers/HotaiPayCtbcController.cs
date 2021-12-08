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
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using WebCommon;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayCtbcController : Controller
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRentT"].ConnectionString;
        private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
        string MEMIDNO = "";

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
            string errorcode = "";
            Int64 LogID = 0;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string ID = "";
            List<HotaiCardInfo> Data = new List<HotaiCardInfo>();

            flag = GetIDNOFromToken(AccessToken, LogID, ref ID, ref lstError);
            flag = true;
            if (flag)
            {
                MEMIDNO = ID;
                input.IDNO = "C221120413";
                input.PRGName = "CreditCardChoose";
                flag = getlist.DoQueryCardList(input, ref output, ref errorcode);
                Data = output.CreditCards;
                return View(Data);
            }
            else
            {
                return View(Data);
            }


        }

        public ActionResult ChooseNewCard()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCard()
        {
            var vm = new OFN_HotaiAddCard();
            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiAddCard() 
            { 
                IDNO = MEMIDNO, //,"C221120413"
                RedirectURL = "https://irentcar.com.tw/", 
                insUser = "TangWeiChi", 
                LogID = 0, 
                PRGName = "AddCard"
            };
            var output = new OFN_HotaiAddCard();

            var errCode = "";
            var flag = hotaipayService.DoAddCard(input, ref output, ref errCode);

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
                insUser= "TangWeiChi",
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


        public bool GetIDNOFromToken(string Access_Token, Int64 LogID, ref string IDNO, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            string CheckTokenName = "usp_CheckTokenReturnID";
            SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
            {
                LogID = LogID,
                Token = Access_Token
            };
            SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
            SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            //checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            if (flag)
            {
                IDNO = spOut.IDNO;
            }
            return flag;
        }
    }
}