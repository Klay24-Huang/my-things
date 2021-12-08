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
using NLog;
using Domain.SP.Input.Hotai;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayCtbcController : Controller
    {

        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
                IDNO = "A227548440",//MEMIDNO,
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


        #region 選擇新的信用卡
        public ActionResult BindNewCard()
        {
            return View();
        }
        #endregion

        #region 開始綁定信用卡
        public ActionResult CreditStart()
        {
            ViewBag.HotaiAccessToken = Session["hotai_access_token"].ToString().Trim();
            return View();
        }
        #endregion


        #region 無信用卡列表頁面 
        public ActionResult NoCreditCard(string irent_access_token)
        {
            HotaipayService HPServices = new HotaipayService();
            bool flag = false;
            string errCode = "";
            string PRGName = "NoCreditCard";
            List<ErrorInfo> errList = new List<ErrorInfo>();
            var IDNO = "";


            if (!string.IsNullOrEmpty(Request.QueryString["irent_access_token"]))
            {
                flag = GetIDNOFromToken(Request.QueryString["irent_access_token"].Trim(), 8514, ref IDNO, ref errList);
                System.Web.HttpContext.Current.Session["IDNO"] = IDNO;

                //將Session賦予值
                if (System.Web.HttpContext.Current.Session["irent_access_token"] == null)
                    System.Web.HttpContext.Current.Session["irent_access_token"] = Request.QueryString["irent_access_token"];
            }
            else
            {
                flag = GetIDNOFromToken(irent_access_token, 8513, ref IDNO, ref errList);
                System.Web.HttpContext.Current.Session["IDNO"] = IDNO;
            }

            //取得卡片清單
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            if (flag) { 
                input.IDNO = IDNO;
               flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
            }

            //和泰Token失效
            if (errCode == "ERR941")
            {
                logger.Error("HotaiPay.NoCreditCard.DoQueryToken fail");
                return RedirectToRoute("/HotaiPay/Login",new {  irent_access_token = irent_access_token }  );
            }

            if (flag)
            {
                if (output.CreditCards.Count > 0)
                {
                    List<HotaiCardInfo> L_Output = output.CreditCards;
                    if (L_Output.Count > 0)
                        return View("CreditCardChoose", L_Output);
                }
            }
            else
            {
                logger.Error("HotaiPayCtbc.NoCreditCard.DoQueryCardList 查詢卡清單失敗 ERRCODE:" + errCode);
            }
            return View();
        }
        #endregion


        #region 選擇綁定卡片
        [HttpPost]
        public ActionResult CreditcardChoose(FormCollection form)
        {
            string IDNO = System.Web.HttpContext.Current.Session["IDNO"].ToString();
            string irent_access_token = System.Web.HttpContext.Current.Session["irent_access_token"].ToString();
            Boolean flag = true;
            string errCode = "";
            HotaipayService HPServices = new HotaipayService();
            string thatCardValue = form["CreditCardList"].Trim();
            if (thatCardValue != "")
            {
                string[] input = thatCardValue.Split('|');
                string MemberOneID = input[0];
                string CardType = input[1];
                string BankDesc = input[2];
                string CardNumber = input[3];
                string CardToken = input[4];

                var sp_input = new SPInput_SetDefaultCard();
                sp_input.IDNO = IDNO;
                sp_input.OneID = MemberOneID;
                sp_input.CardToken = CardToken;
                sp_input.CardNo = CardNumber;
                sp_input.CardType = CardType;
                sp_input.BankDesc = BankDesc;
                sp_input.PRGName = "CreditcardChoose";

                flag = HPServices.sp_SetDefaultCard(sp_input, ref errCode);
                if (!flag)
                    logger.Error("HotaiPayCtbc.CreditcardChoose.sp_SetDefaultCard 設定預設卡失敗 ERRCODE:" + errCode);
            }
            if (flag)
                return Redirect("/HotaiPay/SuccessBind");
            else
                return Redirect("/HotaiPay/BindCardFailed");
        }
        #endregion



    }
}