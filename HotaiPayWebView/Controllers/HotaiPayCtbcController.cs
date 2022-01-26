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
using Newtonsoft.Json;

namespace HotaiPayWebView.Controllers
{
    public class HotaiPayCtbcController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        private string redirectURL = ConfigurationManager.AppSettings["redirectURL"];
        //private string iRentCarURL = ConfigurationManager.AppSettings["iRentCarURL"];
        //private string localURL = ConfigurationManager.AppSettings["localURL"];

        //private static CommonRepository commonRepository = new CommonRepository(ConfigurationManager.ConnectionStrings["IRent"].ConnectionString);
        //private static string MEMIDNO = "";
        //private static string AToken = "";


        // GET: HotaiPayCtbc
        public ActionResult Index()
        {
            return View();
        }


        //[HttpGet] //沒寫也會判定成get?!
        public ActionResult CreditCardChoose()//string irent_access_token，參數來源可以抓URL的QUERYSTRING和VIEW的
        {
            //logger.Error($"哈哈哈哈哈");
            //不抓QUERYSTRING改抓Session
            string irent_access_token = "";
            HotaipayService HPServices = new HotaipayService();
            var decryptDic = new Dictionary<string, string>();
            if (Session["p"] != null)
            {
                decryptDic = HPServices.QueryStringDecryption(Session["p"].ToString().Trim());
                irent_access_token = Session["irent_access_token"].ToString();//decryptDic["irent_access_token"].Trim();
            }

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
                Session["p"] = Session["p"];
                Session["id"] = ID;
                Session["irent_access_token"] = irent_access_token;

                input.IDNO = ID;
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
                IDNO = Session["id"].ToString(),
                RedirectURL = ConfigurationManager.AppSettings["redirectURL"] + "HotaiPayCtbc/BindResult",
                //RedirectURL = Session["redirectURL"].ToString() + "HotaiPayCtbc/BindResult", 
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
            ViewBag.ID = Session["id"];
            
            List<GetBirthDate> lstdata = new HotaiPayCtbcRepository(connetStr).GetBirthDay(Session["id"].ToString());
            ViewBag.BIRTH = lstdata[0].BD;

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

                //OFN_HotaiFastAddCard vm = new OFN_HotaiFastAddCard();
                IFN_HotaiFastAddCard input = new IFN_HotaiFastAddCard()
                {
                    Birthday = inn.Birthday,
                    IDNO = Session["id"].ToString(),
                    CTBCIDNO = inn.CTBCIDNO,//"A121563290"
                    RedirectURL = ConfigurationManager.AppSettings["redirectURL"] + "HotaiPayCtbc/BindResult",
                    //RedirectURL = Session["redirectURL"].ToString() + "HotaiPayCtbc/BindResult", 
                    insUser = "HIMS",
                    LogID = 0,
                    PRGName = "InsPersonInfo"
                };
                string errCode = "";
                //logger.Info($"tanginput : {JsonConvert.SerializeObject(input)}");
                flag = addcard.DoFastAddCard(input, ref output, ref errCode);
                if (flag)
                {
                    //vm = output;               
                    //return Json(vm);
                    return Json(output);
                }
                else
                {
                    //vm.succ = false;
                    output.succ = false;
                    //vm.gotoUrl = errCode;
                    //ViewData["ERROR"] = "ERROR";
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

            @ViewBag.reURL = ConfigurationManager.AppSettings["redirectURL"];//Session["redirectURL"].ToString();


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
        public ActionResult NoCreditCard()
        {
            logger.Error($"NoCreditCard帶入的網址參數 ={Request.QueryString["p"]}");
            logger.Error($"NoCreditCard抓到的session ={Session["p"]}");

            string irent_access_token = "";
            HotaipayService HPServices = new HotaipayService();
            bool flag = false;
            string errCode = "";
            List<ErrorInfo> errList = new List<ErrorInfo>();
            string IDNO = "";
            long LogID = 65471;
            var decryptDic = new Dictionary<string, string>();

            //20220107唐加，反正url會帶p進來，就再解析一次，看看能否解決session機制在webview上的問題
            //若從login轉此頁不會帶p，但app上的按鈕直接進此頁會帶p
            //if (Request.QueryString["p"] != null && Session["p"]==null)
            //{
            //    decryptDic = HPServices.QueryStringDecryption(Request.QueryString["p"].Trim());
            //    Session["p"] = Request.QueryString["p"].Trim();
            //}
            //若有p加密字串

            if (Request.QueryString["p"] != null && Request.QueryString["p"].Trim().Length > 0)
            {
                //重新賦值
                Session["p"] = Request.QueryString["p"];
                //進行解密
                decryptDic = HPServices.QueryStringDecryption(Request.QueryString["p"].Trim());
                irent_access_token = decryptDic["irent_access_token"].Trim();
            }

            if (Session["p"] != null && Session["p"].ToString().Trim().Length > 0)
            {
                Session["p"] = Session["p"].ToString();
                decryptDic = HPServices.QueryStringDecryption(Session["p"].ToString().Trim());
                irent_access_token = decryptDic["irent_access_token"].Trim();
            }

            //try//for DeBug
            //{
            //    foreach (KeyValuePair<string, string> kvp in decryptDic)
            //    {
            //        logger.Debug("Key = {0}, Value = {1}",
            //            kvp.Key, kvp.Value);
            //    }
            //}
            //catch (Exception e) { logger.Error(e.Message); }

            //else {
            //    return View("Login","HotaiPay");
            //}


            //以上面獲得的Token取IDNO
            flag = GetIDNOFromToken(irent_access_token, LogID, ref IDNO, ref errList);

            if (flag)
            {
                Session["id"] = IDNO;
                Session["irent_access_token"] = irent_access_token;
                Session["p"] = Session["p"];
            }
            else
            {
                ViewBag.Alert = "fail to get user info";
                return View();
            }


            //取得卡片清單
            IFN_QueryCardList input = new IFN_QueryCardList();
            OFN_HotaiCreditCardList output = new OFN_HotaiCreditCardList();
            if (flag)
            {
                input.IDNO = IDNO;
                flag = HPServices.DoQueryCardList(input, ref output, ref errCode);
                //logger.Info($"DoQueryCardList |IDNO :{IDNO} | flag:{flag} | output.CreditCards.Count :{output.CreditCards.Count} ");
            }

            var redirectURL = "";
            var nowDomain = Request.Url.AbsoluteUri;
            if (nowDomain.IndexOf("hotaictbc.irentcar.com.tw") != -1)
            {
                redirectURL = this.redirectURL;
            }
            else if (nowDomain.IndexOf("hieasyrent.hotaimotor.com.tw") != -1)
            {
                redirectURL = this.redirectURL;
            }
            //else if (nowDomain.IndexOf("www.irentcar.com.tw") != -1)
            //{
            //    redirectURL = this.iRentCarURL;
            //}
            //else if (nowDomain.IndexOf("localhost:44330/") != -1)
            //{
            //    redirectURL = this.localURL;
            //}
            //Session["redirectURL"] = redirectURL;
            @ViewBag.reURL = redirectURL;


            //和泰Token失效
            if (errCode == "ERR941")
            {
                //logger.Error("HotaiPay.NoCreditCard.DoQueryToken fail");
                return RedirectToRoute("Login", "HotaiPay" );
            }

            if (flag)
            {
                if (output.CreditCards.Count > 0)
                {
                    List<HotaiCardInfo> L_Output = output.CreditCards;
                    if (L_Output.Count > 0)
                    {
                        //return View("CreditCardChoose", L_Output);
                        return RedirectToAction("CreditCardChoose", "HotaiPayCtbc");//,new { form = L_Output }
                    }
                }
            }
            else
            {
                logger.Error($"HotaiPayCtbc.NoCreditCard.DoQueryCardList 查詢卡清單失敗\n ERRCODE={errCode} \n irent_access_token ={irent_access_token}");
            }
            return View();
        }
        #endregion

        #region 選擇綁定卡片
        [HttpPost]
        public ActionResult CreditcardChoose(FormCollection form)
        {
            logger.Info($"CreditcardChoose | Init | form : {JsonConvert.SerializeObject(form)}");
            string IDNO = "";
            if (Session["id"].ToString() != null)
                IDNO = Session["id"].ToString();
            string irent_access_token = Session["irent_access_token"].ToString();
            Boolean flag = true;
            string errCode = "";
            HotaipayService HPServices = new HotaipayService();
            string thatCardValue = form["CreditCardList"].Trim();
            if (thatCardValue != "")
            {
                logger.Info($"選擇的卡片是：\nIDNO={IDNO}\nthatCardValue={thatCardValue}");
                string[] input = thatCardValue.Split('|');
                string MemberOneID = input[0];
                string CardType = input[1];
                string BankDesc = input[2];
                string CardNumber = input[3];
                string CardToken = input[4];
                string BankCode = input[5];

                var sp_input = new SPInput_SetDefaultCard();
                sp_input.IDNO = IDNO;
                sp_input.OneID = MemberOneID;
                sp_input.CardToken = CardToken;
                sp_input.CardNo = CardNumber;
                sp_input.CardType = CardType;
                sp_input.BankDesc = BankDesc;
                sp_input.PRGName = "CreditcardChoose";
                sp_input.BankCode = BankCode;
                logger.Info($"選擇的卡片是：\nIDNO={IDNO}\nOneID={MemberOneID}\nCardToken={CardToken}\nCardNo={CardNumber}\nCardType={CardType}\nBankDesc={BankDesc}\nBankCode={BankCode} ");
                flag = HPServices.sp_SetDefaultCard(sp_input, ref errCode);
                if (!flag)
                {
                    logger.Error($"HotaiPayCtbc.CreditcardChoose.sp_SetDefaultCard 設定預設卡失敗IDNO={IDNO} ERRCODE= {errCode}");
                    ViewBag.Alert = "fail to get update database";
                }
            }
            else
            {
                return View("NoCreditCard", irent_access_token);
            }
            if (flag)
                return RedirectToRoute(new { controller = "HotaiPayCtbc", action = "SuccessBind" });//return Redirect("/irweb/HotaiPayCtbc/SuccessBind");
            else
                return RedirectToRoute(new { controller = "HotaiPay", action = "BindCardFailed" }); //return Redirect("/irweb/HotaiPayCtbc/BindCardFailed");
        }
        #endregion

        public ActionResult BindResult(string StatusCode, string StatusDesc)
        {
            logger.Info($"tanginput | BindResult | StatusCode : {StatusCode} | StatusDesc : {StatusDesc}");
            
            //string a = StatusCode;
            //string b = StatusDesc;
            if (StatusDesc.ToUpper() == "SUCCESS")
            {
                return RedirectToAction("NoCreditCard", "HotaiPayCtbc", new { p = Session["p"] }); //irent_access_token = Session["irent_access_token"].ToString()
                //return RedirectToAction("NoCreditCard", "HotaiPayCtbc", new { irent_access_token = AT });
            }
            else
            {
                //return RedirectToAction("BindCardFailed", "HotaiPay",new { a=1}); //造成login時說BindCardFailed有兩個無法分辨
                return RedirectToAction("BindCardFailed", "HotaiPay");
            }
        }

    }
}