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
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        //private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
        private static string MEMIDNO = "";
        private static string AToken = "";

        // GET: HotaiPayCtbc
        public ActionResult Index()
        {
            return View();
        }

        //有一樣的了
        //[HttpPost]
        //public ActionResult CreditCardChoose()
        //{
        //    bool flag;
        //    HotaipayService hotaipayService = new HotaipayService();
        //    SPInput_SetDefaultCard spInput = new SPInput_SetDefaultCard();
        //    string errCode="";
        //    spInput.IDNO = MEMIDNO;
        //    flag = hotaipayService.sp_SetDefaultCard(spInput,ref errCode);
        //    if (flag)
        //    {
        //        return RedirectToAction("SuccessBind", "HotaiPay");
        //    }
        //    else
        //    {
        //        return RedirectToAction("BindCardFailed", "HotaiPay");
        //    }
        //}

        //[HttpGet] //沒寫也會判定成get?!
        public ActionResult CreditCardChoose(string irent_access_token)//參數來源可以抓URL的QUERYSTRING和VIEW的
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

            flag = GetIDNOFromToken(irent_access_token, LogID, ref ID, ref lstError);
            //flag = true;
            if (flag)
            {
                MEMIDNO = ID; //"A227548440";
                AToken = irent_access_token;
                input.IDNO = ID;// "C221120413";
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

        //同BindNewCard
        //public ActionResult ChooseNewCard()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult AddCard()
        {
            var vm = new OFN_HotaiAddCard();
            HotaipayService hotaipayService = new HotaipayService();

            var input = new IFN_HotaiAddCard()
            {
                IDNO = MEMIDNO,//"A227548440",
                RedirectURL = "https://www.irentcar.com.tw/irweb/HotaiPayCtbc/BindResult",
                //RedirectURL = "https://www.irentcar.com.tw",
                insUser = "TangWeiChi",
                LogID = 0,
                PRGName = "AddCard"
            };
            var output = new OFN_HotaiAddCard();

            var errCode = "";
            var flag = hotaipayService.DoAddCard(input, ref output, ref errCode);
            //System.Web.HttpContext.Current.Session["IDNO"]

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
            if (ModelState.IsValid)
            {
                bool flag;
                HotaipayService addcard = new HotaipayService();
                OFN_HotaiFastAddCard output = new OFN_HotaiFastAddCard();

                OFN_HotaiFastAddCard vm = new OFN_HotaiFastAddCard();

                //IFN_HotaiFastAddCard input = new IFN_HotaiFastAddCard();
                //input.Birthday = BIRTHDATE;
                IFN_HotaiFastAddCard input = new IFN_HotaiFastAddCard()
                {
                    Birthday = inn.Birthday,//"19910804"
                    IDNO = inn.CTBCIDNO,//"A121563290"
                    CTBCIDNO = inn.CTBCIDNO,//"A121563290"
                    RedirectURL = "https://www.irentcar.com.tw/irweb/HotaiPayCtbc/BindResult",
                    //RedirectURL = "https://www.irentcar.com.tw",
                    insUser = "TangWeiChi",
                    LogID = 0,
                    PRGName = "InsPersonInfo"
                };
                string errCode = "";
                flag = addcard.DoFastAddCard(input, ref output, ref errCode);
                if (flag)
                {
                    vm = output;
                    return Json(vm);
                }
                else
                {
                    vm.succ = false;
                    ViewData["ERROR"] = "ERROR";
                    return View();
                }
            }
            else
            {
                //ViewBag.CTBCIDNO = inn.CTBCIDNO;
                //ViewBag.Birthday = inn.Birthday;
                return View();
            }
            //return Json(vm);
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
        //app會呼叫這支，帶入accesstoken
        public ActionResult BindNewCard(string irent_access_token)
        {
            //string a = irent_access_token;
            return View();
        }
        #endregion

        #region 開始綁定信用卡
        public ActionResult CreditStart(string irent_access_token)
        {
            Session["irent_access_token"] = irent_access_token;
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
                //若沒有傳值則從網址列帶參數 --上正式需+串解密
                if (string.IsNullOrEmpty(irent_access_token))
                {
                    irent_access_token = Request.QueryString["irent_access_token"].Trim();
                }
                //else{} 沒收到任何token

                flag = GetIDNOFromToken(irent_access_token, 8514, ref IDNO, ref errList);
                System.Web.HttpContext.Current.Session["IDNO"] = IDNO;
                MEMIDNO = IDNO;
                AToken = irent_access_token;

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
            if (flag)
            {
                input.IDNO = IDNO;
                flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
            }

            //和泰Token失效
            if (errCode == "ERR941")
            {
                logger.Error("HotaiPay.NoCreditCard.DoQueryToken fail");
                return RedirectToRoute("/HotaiPay/Login", new { irent_access_token = irent_access_token });
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
                return Redirect("/HotaiPayCtbc/SuccessBind");
            else
                return Redirect("/HotaiPayCtbc/BindCardFailed");
        }
        #endregion

        public ActionResult BindResult(string StatusCode, string StatusDesc)
        {
            //string a = StatusCode;
            //string b = StatusDesc;
            if (StatusCode == "I0000" && StatusDesc == "SUCCESS")
            {
                return RedirectToAction("CreditCardChoose", "HotaiPayCtbc", new { irent_access_token = AToken });
            }
            else
            {
                //return RedirectToAction("BindCardFailed", "HotaiPay",new { a=1}); //造成login時說BindCardFailed有兩個無法分辨
                return RedirectToAction("BindCardFailed", "HotaiPay");
            }
        }

    }
}