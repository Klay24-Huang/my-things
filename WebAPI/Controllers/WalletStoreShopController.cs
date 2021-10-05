using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet.Param;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包儲值-超商條碼
    /// </summary>
    public class WalletStoreShopController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinCID = ConfigurationManager.AppSettings["TaishinCID"].ToString();                 //用戶代碼Base64
        private string GetBarCode = ConfigurationManager.AppSettings["GetBarCode"].ToString();                 //超商繳費條碼查詢
        private string GetBarCodeShortUrl = ConfigurationManager.AppSettings["GetBarCodeShortUrl"].ToString(); //超商繳費條碼短網址查詢
        private string CreateCvsPayInfo = ConfigurationManager.AppSettings["CreateCvsPayInfo"].ToString();     //超商繳費資訊上傳-新增

        protected static Logger logger = LogManager.GetCurrentClassLogger();


        [HttpPost]
        public Dictionary<string, object> DoWalletStoreShop(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var carRentComm = new CarRentCommon();
            var wsp = new WalletSp();
            var walletService = new WalletService();

            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoreShop";
            string TradeType = "Store_Shop"; ///交易類別
            string PRGID = "222"; //APIId

            Int64 LogID = 0;
            Int16 ErrType = 0;

            TaishinWallet WalletAPI = new TaishinWallet();
            IAPI_WalletStoreShop apiInput = null;
            var apiOutput = new OAPI_WalletStoreShop();

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string cvsPayToken = "";
            string cvsCode = "";
            string paymentId = "";
            string hmacVal = "";

            #endregion
            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                flag = true;
                if (flag)
                {
                    //寫入API Log
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoreShop>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (apiInput.StoreMoney <= 0 || apiInput.CvsType < 0 && apiInput.CvsType > 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (apiInput.StoreMoney > 0 && apiInput.StoreMoney < 300)
                    {
                        flag = false;
                        errCode = "ERR284";
                    }
                    else if (apiInput.StoreMoney > 20000)
                    {
                        flag = false;
                        errCode = "ERR285";
                    }

                    //不開放訪客
                    if (isGuest)
                    {
                        flag = false;
                        errCode = "ERR101";
                    }
                    trace.FlowList.Add("防呆");
                }
                #endregion

                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);

                }
                #endregion

                #region 儲值金額限制檢核
                if (flag)
                {
                    flag = walletService.CheckStoreAmtLimit(apiInput.StoreMoney, IDNO, LogID, Access_Token, ref flag, ref errCode);
                }
                #endregion

                #region 取得台新APIToken
                if (flag)
                {
                    WebAPIOutput_GetTaishinCvsPayToken output_GetTaishinCvsPayToken = new WebAPIOutput_GetTaishinCvsPayToken();
                    flag = WalletAPI.DoGetTaishinCvsPayToken(ref output_GetTaishinCvsPayToken);
                    if (flag && output_GetTaishinCvsPayToken != null)
                    {
                        cvsPayToken = output_GetTaishinCvsPayToken.access_token;
                    }
                    else
                    {
                        errCode = "ERR939";
                        errMsg = "超商條碼Token產生失敗，請洽系統管理員";
                    }

                    trace.traceAdd("DoGetTaishinCvsPayToken", new { output_GetTaishinCvsPayToken });
                    trace.FlowList.Add("取得台新APIToken");
                }
                #endregion

                DateTime NowTime = DateTime.Now;
                DateTime dueDate = NowTime.Date.AddDays(2).AddSeconds(-1);

                #region 產生超商銷帳編號
                if (flag)
                {
                    string cvsIdentifier = GetCvsCode(apiInput.CvsType).Item3; //超商業者識別碼
                    SPInput_GetCvsPaymentId spInput_GetCvsPaymentId = new SPInput_GetCvsPaymentId()
                    {
                        IDNO = IDNO,
                        Token = Access_Token,
                        LogID = LogID,
                        CvsIdentifier = cvsIdentifier
                    };

                    SPOutput_GetCvsPaymentId spOutput_GetCvsPaymentId = wsp.sp_GetCvsPaymentId(spInput_GetCvsPaymentId, ref errCode);
                    if (!string.IsNullOrWhiteSpace(spOutput_GetCvsPaymentId.PaymentId))
                    {
                        paymentId = spOutput_GetCvsPaymentId.PaymentId;
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR938";
                        errMsg = "銷帳編號產生失敗，請洽系統管理員";
                    }

                    trace.traceAdd("sp_GetCvsPaymentId", new { spOutput_GetCvsPaymentId });
                    trace.FlowList.Add("產生銷帳編號");
                }
                #endregion

                #region 超商繳費資訊上傳-新增
                if (flag)
                {
                    cvsCode = GetCvsCode(apiInput.CvsType).Item2; //超商代收代號
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    IHUBReqHeader header = new IHUBReqHeader()
                    {
                        cTxSn = guid,
                        txDate = string.Format("{0:yyyyMMddTHHmmssK}", NowTime).Replace(":", ""),
                        txId = CreateCvsPayInfo,
                        cId = TaishinCID,
                    };


                    List<CVSPayInfoDetailReq> payInfoList = new List<CVSPayInfoDetailReq>();
                    CVSPayInfoDetailReq detailReq = new CVSPayInfoDetailReq
                    {
                        paidDue = dueDate.ToString("yyyyMMdd"),
                        paymentId = paymentId,
                        payAmount = apiInput.StoreMoney,
                        payPeriod = 1,
                        overPaid = "N",
                        custId = IDNO,
                        custMobile = "",
                        custEmail = "",
                        memo = ""
                    };
                    payInfoList.Add(detailReq);


                    WebAPI_CreateCvsPayInfo webAPI_CreateCvsPayInfo = new WebAPI_CreateCvsPayInfo()
                    {
                        header = header,
                        body = new CvsPayInfoReq()
                        {
                            txType = "i",
                            txDate = NowTime.ToString("yyyyMMdd"),
                            txBatchNo = NowTime.ToString("yyyyMMddHHmmssfff"),
                            recordCount = 1,
                            totalAmount = apiInput.StoreMoney,
                            cvsCode = new CvsCode { cvsCode = cvsCode, cvsType = apiInput.CvsType },
                            detail = payInfoList
                        }
                    };

                    hmacVal =WalletAPI.GetHmacVal(webAPI_CreateCvsPayInfo, webAPI_CreateCvsPayInfo.header.cTxSn);

                    WebAPIOutput_CreateCvsPayInfo output_CreateCvsPayInfo = new WebAPIOutput_CreateCvsPayInfo();
                    flag = WalletAPI.DoStoreShopCreateCvsPayInfo(webAPI_CreateCvsPayInfo, funName, cvsPayToken, hmacVal, ref errCode, ref output_CreateCvsPayInfo);
                    if (!flag || output_CreateCvsPayInfo == null)
                    {
                        errCode = "ERR941";
                        errMsg = "超商繳費資訊新增失敗，請洽系統管理員";
                    }
                }
                #endregion

                #region Barcode查詢
                if (flag)
                {
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    IHUBReqHeader header = new IHUBReqHeader()
                    {
                        cTxSn = guid,
                        txDate = string.Format("{0:yyyyMMddTHHmmssK}", NowTime).Replace(":", ""),
                        txId = GetBarCode,
                        cId = TaishinCID,
                    };

                    WebAPI_GetBarcode webAPI_GetBarcode = new WebAPI_GetBarcode()
                    {
                        header = header,
                        body = new BarcodeReq()
                        {
                            cvsCode = cvsCode,
                            dueDate = dueDate.ToString("yyyyMMdd"),
                            cvsType = apiInput.CvsType,
                            payAmount = apiInput.StoreMoney,
                            paymentId = paymentId, 
                            memo = "",
                            payPeriod = 1
                        }
                    };

                    hmacVal = WalletAPI.GetHmacVal(webAPI_GetBarcode, webAPI_GetBarcode.header.cTxSn);

                    WebAPIOutput_GetBarCode outGetBarCode = new WebAPIOutput_GetBarCode();
                    flag = WalletAPI.DoStoreShopGetBarcode(webAPI_GetBarcode, funName, cvsPayToken, hmacVal, ref errCode, ref outGetBarCode);
                    if (flag && outGetBarCode != null)
                    {
                        apiOutput.StoreMoney = apiInput.StoreMoney;
                        apiOutput.Barcode64 = outGetBarCode.body.barcode64;
                        apiOutput.ShopBarCode1 = outGetBarCode.body.code1;
                        apiOutput.ShopBarCode2 = outGetBarCode.body.code2;
                        apiOutput.ShopBarCode3 = outGetBarCode.body.code3;
                        apiOutput.PayDeadline = dueDate.ToString("yyyy/MM/dd 23:59:59");
                    }
                    else
                    {
                        errCode = "ERR940";
                        errMsg = "超商條碼產生失敗，請洽系統管理員";
                    }

                    trace.traceAdd("DoStoreShopGetBarcode", new { webAPI_GetBarcode, cvsPayToken, hmacVal, errCode, outGetBarCode });
                    trace.FlowList.Add("台新回傳Barcode");
                }
                #endregion

                apiOutput.StroeResult = flag ? 1 : 0;

                trace.traceAdd("TraceFinal", new { errCode, errMsg });
                carRepo.AddTraceLog(Convert.ToInt32(PRGID), funName, trace, flag);
            }
            catch (Exception ex)
            {
                flag = false;
                apiOutput.StroeResult = 0;
                trace.BaseMsg = ex.Message;
            }

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion

        }


        private List<Tuple<int, string, string>> CvsCodeList = new List<Tuple<int, string, string>>()
        {
            //超商類型(0:7-11 1:全家),超商代收代號,業者識別碼
            new Tuple<int, string,string>(0,"E6H","IRS"),
            new Tuple<int, string,string>(1,"K9A","IRF"),
        };

        private Tuple<int, string, string> GetCvsCode(int CvsType)
        {
            return CvsCodeList.Where(x => x.Item1 == CvsType).FirstOrDefault();
        }


     
    }
}
