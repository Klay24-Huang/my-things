using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Bill;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Domain.WebAPI.output.Taishin;
using Domain.SP.Input.Subscription;
using Domain.SP.Output.Subscription;
using WebAPI.Models.Param.CusFun.Input;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using NLog;
using System.Text;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 立即購買-工具跳過信用卡付費
    /// </summary>
    public class BuyNowToolController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();


        #region 月租購買(使用情境 1.客人已刷卡未建訂閱制月租資料、補履保發票)
        [Route("api/BuyNowTool/DoAddMonth")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoAddMonth([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon
            {
                ApiID = 179
            };
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNowTool_DoAddMonth";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNowTool_AddMonth();
            var outputApi = new OAPI_BuyNow_Base();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int ProdPrice = 0;
            int MonthlyRentId = 0;
            int IsMoto = 0;
            string TransactionNo = "";
            string CreditCardNo = "";
            string AuthCode = "";
            string MerchantTradeNo = "";
            string INVNO = "";
            string SDate = "";
            string EDate = "";
            int nowPeriod = 0;


            var mem = new Domain.MemberData.RegisterData();
            var InvData = new Domain.MemberData.InvoiceData();

            HiEasyRentAPI EasyRentApi = new HiEasyRentAPI();
            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNowTool_AddMonth>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) || apiInput.MonProPeriod == 0 || string.IsNullOrWhiteSpace(apiInput.IDNO))
                        {
                            flag = false;
                            errCode = "ERR900";
                            errMsg = "MonProjID、MonProPeriod、IDNO必填";
                        }
                    }

                    if (flag)
                    {
                        //不刷卡代表只補資料(需帶信用卡交易紀錄)
                        if (apiInput.CreditCardFlg == 0 && (string.IsNullOrWhiteSpace(apiInput.MerchantTradeNo) || string.IsNullOrWhiteSpace(apiInput.TransactionNo)))
                        {
                            flag = false;
                            errCode = "ERR247";
                            errMsg = "MerchantTradeNo、TransactionNo必填";
                        }
                    }

                    if (flag)
                    {
                        //沒有要產生訂閱制資料，MonthlyRentId為必填
                        if (apiInput.SubsDataFlg == 0 && apiInput.MonthlyRentId == 0)
                        {
                            flag = false;
                            errCode = "ERR247";
                            errMsg = "MonthlyRentId必填";
                        }
                    }

                    if (flag)
                    {
                        MerchantTradeNo = apiInput.MerchantTradeNo;
                        TransactionNo = apiInput.TransactionNo;
                        MonthlyRentId = apiInput.MonthlyRentId;
                        IDNO = apiInput.IDNO;
                        ProdPrice = apiInput.ProdPrice; //補資料從APIInput帶入
                    }

                    trace.traceAdd("foolproof", new { flag, apiInput, errCode, errMsg });
                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷(註解)
                //if (flag && isGuest == false)
                //{
                //    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                //    buyNxtCom.IDNO = IDNO;
                //    trace.FlowList.Add("Token判斷");
                //}
                #endregion

                #region TB

                #region 補資料從MonthlyRentId取當期期數
                if (apiInput.SubsDataFlg != 1 && flag) 
                {
                    var nowPeriodMonData = msp.Sql_GetMonData(apiInput.MonthlyRentId);
                    SDate = Convert.ToDateTime(nowPeriodMonData["StartDate"]).ToString("yyyyMMdd");
                    EDate = Convert.ToDateTime(nowPeriodMonData["EndDate"]).ToString("yyyyMMdd");
                    nowPeriod = Convert.ToInt32(nowPeriodMonData["NowPeriod"]);
                    string nowIDNO = nowPeriodMonData["IDNO"].ToString();

                    if (IDNO != nowIDNO)
                    {
                        errCode = "ERR247";
                        errMsg = "傳入的IDNO與合約不符";
                    }
                }
                #endregion

                #region 購買前檢核
                if (apiInput.SubsDataFlg == 1 && flag)
                {
                    string spErrCode = "";
                    var spIn = new SPInput_BuyNowAddMonth_Q01()
                    {
                        IDNO = IDNO,
                        MonProjId = apiInput.MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays,
                        LogID = LogID,
                        VerifyType = 0
                    };
                    if (!msp.sp_BuyNowAddMonth_Q01(spIn, ref spErrCode))
                    {
                        flag = false;
                        errCode = spErrCode;
                        errMsg = "購買失敗!";
                    }
                    trace.traceAdd("BuyNowAddMonth", new { spIn, spErrCode });
                    trace.FlowList.Add("購買前檢核");
                }
                #endregion

                #region 後續流程
                if (flag)
                {
                    #region 載入後續Api所需資料

                    buyNxtCom.IDNO = IDNO;
                    InvData = mscom.GetINVDataFromMember(IDNO);
                    Int64 InvoTypeId = InvData.InvocieTypeId;

                    buyNxtCom.LogID = LogID;
                    var objJson = new
                    {
                        apiInput.MonProjID,
                        apiInput.MonProPeriod,
                        apiInput.ShortDays,
                        apiInput.SetSubsNxt
                    };
                    buyNxtCom.ApiJson = JsonConvert.SerializeObject(objJson);
                    buyNxtCom.PayTypeId = 5;//TB_CodeId=5目前只有信用卡付款
                    buyNxtCom.InvoTypeId = InvoTypeId;
                    flag = buyNxtCom.CkApiID();
                    errCode = buyNxtCom.errCode;

                    #endregion

                    #region 取得月租設定資訊
                    if (flag)
                    {
                        string sp_errCode = "";
                        var spin = new SPInput_GetMonSetInfo()
                        {
                            LogID = LogID,
                            MonProjID = apiInput.MonProjID,
                            MonProPeriod = apiInput.MonProPeriod,
                            ShortDays = apiInput.ShortDays
                        };

                        int PeriodPrice = 0;
                        var monObjs = msp.sp_GetMonSetInfo(spin, ref sp_errCode);
                        if (monObjs != null && monObjs.Count() > 0)
                        {
                            var fItem = monObjs.FirstOrDefault();
                            if (fItem.PeriodPrice > 0) 
                            {
                                PeriodPrice = fItem.PeriodPrice;
                                IsMoto = fItem.IsMoto;
                            }                             
                        }
                        ProdPrice = apiInput.ProdPrice > 0 ? apiInput.ProdPrice : PeriodPrice; //若APIInput有帶值，則不取月租專案價格
                        trace.traceAdd("monthInfo", new { monObjs, spin, ProdPrice, sp_errCode });
                        trace.FlowList.Add("月租資訊");
                    }

                    #endregion

                    if (ProdPrice > 0)
                    {
                        #region 信用卡交易

                        var WsOut = new WebAPIOutput_Auth();
                        if (apiInput.CreditCardFlg == 1) //有價格才進行信用卡交易
                        {
                            trace.traceAdd("CarTradeIn", new { IDNO, ProdPrice, errCode });

                            try
                            {
                                flag = mscom.Month_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);

                                if (WsOut != null)
                                    trace.traceAdd("CarTradeResult", new { WsOut });

                                if (flag && WsOut.ResponseParams != null)
                                {
                                    TransactionNo = WsOut.ResponseParams.ResultData.ServiceTradeNo;
                                    AuthCode = WsOut.ResponseParams.ResultData.AuthIdResp;
                                    CreditCardNo = WsOut.ResponseParams.ResultData.CardNumber;
                                    MerchantTradeNo = WsOut.ResponseParams.ResultData.MerchantTradeNo;
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                errCode = "ERR270";
                                trace.BaseMsg = ex.Message;
                                throw new Exception("TSIBTrade Fail");

                            }

                            trace.FlowList.Add("信用卡交易");
                        }

                        #endregion

                        #region 後續api處理
                        if (apiInput.SubsDataFlg == 1 && flag)
                        {
                            flag = buyNxtCom.exeNxt(MerchantTradeNo, TransactionNo);
                            errCode = buyNxtCom.errCode;
                            trace.traceAdd("exeNxt", new { buyNxtCom });
                            trace.FlowList.Add("後續api處理");
                        }
                        #endregion

                        #region 履保
                        if (apiInput.EscrowMonthFlg == 1 && flag)
                        {
                            try
                            {
                                string xerrCode = "";
                                string xerrMsg = "";

                                logger.Info("履保開始!");
                                mem = msp.GetMemberData(IDNO, LogID, Access_Token);
                                if (mem != null)
                                {
                                    var spin = new ICF_TSIB_Escrow_Type()
                                    {
                                        IDNO = IDNO,
                                        Name = mem.MEMCNAME,
                                        PhoneNo = mem.MEMTEL,
                                        Email = mem.MEMEMAIL,
                                        Amount = ProdPrice,
                                        UseType = 0,
                                        MonthlyNo = apiInput.MonthlyRentId == 0 ? buyNxtCom.MonthlyRentId : apiInput.MonthlyRentId,
                                        PRGID = funName
                                    };
                                    var xFlag = mscom.TSIB_Escrow_Month(spin, ref xerrCode, ref xerrMsg);
                                    trace.traceAdd("TSIB_Escrow_Month", new { spin, xFlag, xerrCode, xerrMsg });
                                    trace.FlowList.Add("履保處理");
                                }
                            }
                            catch (Exception ex)
                            {
                                trace.traceAdd("Escrow_Exception", new { ex });
                            }
                        }
                        #endregion

                        #region 發票
                        if (apiInput.InvoiceFlg == 1 && flag)
                        {
                            try
                            {
                                WebAPIInput_MonthlyRentSave wsInput = new WebAPIInput_MonthlyRentSave()
                                {
                                    CUSTID = IDNO,
                                    CUSTNM = mem.MEMCNAME,
                                    EMAIL = mem.MEMEMAIL,
                                    MonRentID = apiInput.MonthlyRentId == 0 ? buyNxtCom.MonthlyRentId : apiInput.MonthlyRentId,
                                    MonProjID = apiInput.MonProjID,
                                    MonProPeriod = apiInput.MonProPeriod,
                                    ShortDays = apiInput.ShortDays,
                                    NowPeriod = apiInput.MonthlyRentId == 0 ? buyNxtCom.NowPeriod : nowPeriod,
                                    SDATE = SDate,
                                    EDATE = EDate,
                                    IsMoto = IsMoto,
                                    RCVAMT = ProdPrice,
                                    UNIMNO = InvData.UNIMNO,
                                    CARDNO = CreditCardNo,
                                    AUTHCODE = AuthCode,
                                    NORDNO = TransactionNo,
                                    INVKIND = InvData.InvocieType,
                                    CARRIERID = InvData.CARRIERID,
                                    NPOBAN = InvData.NPOBAN,
                                    INVTITLE = "",
                                    INVADDR = ""
                                };

                                wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>
                            {
                                new NPR130SavePaymentList()
                                {
                                    PAYMENTTYPE = "1",
                                    PAYTYPE = "4",
                                    PAYAMT = ProdPrice,
                                    PORDNO = TransactionNo,
                                    PAYMEMO = "月租訂閱制"
                                }
                            };
                                WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();
                                var xflag = EasyRentApi.MonthlyRentSave(wsInput, ref wsOutput);
                                if (wsOutput.Result == false)
                                {
                                    xflag = false;
                                }
                                else
                                {
                                    INVNO = wsOutput.Data[0].INVNO;
                                    logger.Trace("INVNO=" + INVNO);
                                }
                                trace.traceAdd("MonthlyRentSave", new { wsInput, wsOutput });
                                trace.FlowList.Add("發票開立");
                                if (INVNO != "")
                                {
                                    //寫入發票資訊
                                    string sp_errCode = "";
                                    var spin = new SPInput_SaveInvno()
                                    {
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        NowPeriod = wsInput.NowPeriod,
                                        PayTypeId = (Int64)apiInput.PayTypeId,
                                        InvoTypeId = InvoTypeId,
                                        InvoiceType = InvData.InvocieType,
                                        CARRIERID = InvData.CARRIERID,
                                        UNIMNO = InvData.UNIMNO,
                                        NPOBAN = InvData.NPOBAN,
                                        Invno = INVNO,
                                        InvoicePrice = ProdPrice,
                                        InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                        PRGID = funName,
                                        MonthlyRentID = wsInput.MonRentID
                                    };

                                    xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                                    if (!xflag)
                                    {
                                        logger.Trace("spError=" + sp_errCode);
                                    }
                                    trace.traceAdd("sp_SaveSubsInvno", new { spin, sp_errCode });
                                    trace.FlowList.Add("發票存檔");
                                }
                                else
                                {
                                    //20210826 ADD BY ADAM REASON.發票開立失敗處理
                                    //資料寫入錯誤紀錄log TB_MonthlyInvErrLog
                                    string sp_errCode = "";
                                    var spInput = new SPInput_InsMonthlyInvErr()
                                    {
                                        ApiInput = JsonConvert.SerializeObject(wsInput),
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        MonthlyRentID = wsInput.MonRentID,
                                        MonProjID = wsInput.MonProjID,
                                        MonProPeriod = wsInput.MonProPeriod,
                                        ShortDays = wsInput.ShortDays,
                                        NowPeriod = wsInput.NowPeriod,
                                        PayTypeId = (Int64)apiInput.PayTypeId,
                                        InvoTypeId = InvoTypeId,
                                        InvoiceType = InvData.InvocieType,
                                        CARRIERID = InvData.CARRIERID,
                                        UNIMNO = InvData.UNIMNO,
                                        NPOBAN = InvData.NPOBAN,
                                        INVAMT = ProdPrice,
                                        PRGID = funName,
                                        RtnCode = wsOutput?.RtnCode ?? "-4",
                                        RtnMsg = wsOutput?.Message ?? ""
                                    };

                                    xflag = msp.sp_InsMonthlyInvErr(spInput, ref sp_errCode);
                                    if (!xflag)
                                    {
                                        logger.Trace("spError=" + sp_errCode);
                                    }
                                    trace.traceAdd("sp_InsMonthlyInvErr", new { spInput, sp_errCode });
                                    trace.FlowList.Add("發票錯誤處理");
                                }
                            }
                            catch (Exception ex)
                            {
                                //紀錄開立失敗
                                logger.Trace(ex.Message);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        errCode = "ERR247";
                        errMsg = "輸入參數格式不符";
                    }
                }
                #endregion

                #endregion
                outputApi.PayResult = flag ? 1 : 0;

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(250, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
        #endregion

        #region 月租升轉(使用情境 1.客人已刷卡未建訂閱制升轉資料 2.只跑履保發票用DoAddMonth)
        [Route("api/BuyNowTool/DoUpMonth")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoUpMonth([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon
            {
                ApiID = 188
            };
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNowTool_DoUpMonth";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNowTool_DoUpMonth();
            var outputApi = new OAPI_BuyNow_Base();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int ProdPrice_ori = 0;
            int ProdPrice_nxt = 0;
            int ProdPrice = 0;
            int IsMoto = 0;
            string TransactionNo = "";
            string CreditCardNo = "";
            string AuthCode = "";
            string MerchantTradeNo = "";
            string INVNO = "";
            var mem = new Domain.MemberData.RegisterData();
            var InvData = new Domain.MemberData.InvoiceData();

            HiEasyRentAPI EasyRentApi = new HiEasyRentAPI();
            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNowTool_DoUpMonth>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) || apiInput.MonProPeriod == 0 || string.IsNullOrWhiteSpace(apiInput.IDNO))
                        {
                            flag = false;
                            errCode = "ERR900";
                            errMsg = "MonProjID、MonProPeriod、IDNO必填";
                        }
                    }

                    if (flag)
                    {
                        //不刷卡代表只補資料(需帶當初信用卡交易紀錄)
                        if (apiInput.CreditCardFlg == 0 && (string.IsNullOrWhiteSpace(apiInput.MerchantTradeNo) || string.IsNullOrWhiteSpace(apiInput.TransactionNo)))
                        {
                            flag = false;
                            errCode = "ERR247";
                            errMsg = "MerchantTradeNo、TransactionNo必填";
                        }
                    }

                    if (flag)
                    {
                        MerchantTradeNo = apiInput.MerchantTradeNo;
                        TransactionNo = apiInput.TransactionNo;
                        IDNO = apiInput.IDNO;
                    }

                    trace.traceAdd("foolproof", new { flag, apiInput, errCode, errMsg });
                    trace.FlowList.Add("防呆");

                }

                #endregion


                #region TB

                #region 升轉前檢核
                if (flag)
                {
                    string spErrCode = "";
                    var spIn = new SPInput_BuyNowAddMonth_Q01()
                    {
                        IDNO = IDNO,
                        MonProjId = apiInput.MonProjID,
                        UP_MonProjID = apiInput.UP_MonProjID,
                        MonProPeriod = apiInput.MonProPeriod,
                        UP_MonProPeriod = apiInput.UP_MonProPeriod,
                        ShortDays = apiInput.ShortDays,
                        UP_ShortDays = apiInput.UP_ShortDays,
                        LogID = LogID,
                        VerifyType = 1
                    };
                    if (!msp.sp_BuyNowAddMonth_Q01(spIn, ref spErrCode))
                    {
                        flag = false;
                        errCode = spErrCode;
                    }
                    trace.traceAdd("BuyNowAddMonth", new { spIn, spErrCode });
                    trace.FlowList.Add("升轉前檢核");
                }
                #endregion

                #region 進入升轉流程
                if (flag)
                {
                    #region 載入後續Api所需資料

                    buyNxtCom.IDNO = IDNO;
                    InvData = mscom.GetINVDataFromMember(IDNO);
                    Int64 InvoTypeId = InvData.InvocieTypeId;

                    buyNxtCom.LogID = LogID;
                    var objJson = new
                    {
                        apiInput.MonProjID,
                        apiInput.MonProPeriod,
                        apiInput.ShortDays,
                        apiInput.UP_MonProjID,
                        apiInput.UP_MonProPeriod,
                        apiInput.UP_ShortDays
                    };
                    buyNxtCom.ApiJson = JsonConvert.SerializeObject(objJson);
                    buyNxtCom.PayTypeId = 5;//TB_CodeId=5目前只有信用卡付款
                    buyNxtCom.InvoTypeId = InvoTypeId;
                    flag = buyNxtCom.CkApiID();
                    errCode = buyNxtCom.errCode;

                    #endregion

                    #region 取得月租設定資訊

                    if (flag)
                    {
                        string sp_errCode = "";
                        var spin_ori = new SPInput_GetMonSetInfo()
                        {
                            LogID = LogID,
                            MonProjID = apiInput.MonProjID,
                            MonProPeriod = apiInput.MonProPeriod,
                            ShortDays = apiInput.ShortDays
                        };

                        var monObjs_ori = msp.sp_GetMonSetInfo(spin_ori, ref sp_errCode);
                        if (monObjs_ori != null && monObjs_ori.Count() > 0)
                        {
                            var fItem = monObjs_ori.FirstOrDefault();
                            if (fItem.PeriodPrice > 0)
                                ProdPrice_ori = fItem.PeriodPrice;
                        }

                        var spin_nxt = new SPInput_GetMonSetInfo()
                        {
                            LogID = LogID,
                            MonProjID = apiInput.UP_MonProjID,
                            MonProPeriod = apiInput.UP_MonProPeriod,
                            ShortDays = apiInput.UP_ShortDays
                        };
                        var monObjs_nxt = msp.sp_GetMonSetInfo(spin_nxt, ref sp_errCode);
                        if (monObjs_nxt != null && monObjs_nxt.Count() > 0)
                        {
                            var fItem = monObjs_nxt.FirstOrDefault();
                            if (fItem.PeriodPrice > 0)
                                ProdPrice_nxt = fItem.PeriodPrice;

                            IsMoto = fItem.IsMoto;
                        }
                        ProdPrice = ProdPrice_nxt - ProdPrice_ori;
                        ProdPrice = ProdPrice > 0 ? ProdPrice : 0;
                    }

                    #endregion

                    if (ProdPrice > 0) //有價格才往下做
                    {
                        #region 信用卡交易

                        var WsOut = new WebAPIOutput_Auth();
                        if (apiInput.CreditCardFlg == 1) //有價格才進行信用卡交易
                        {
                            trace.traceAdd("CarTradeIn", new { IDNO, ProdPrice, errCode });

                            #region 刷卡狀態-共用

                            string spErrCode = "";
                            var spIn = new SPInput_SetSubsCreditStatus()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
                                APIID = 181,
                                ActionNM = funName,
                                ApiCallKey = JsonConvert.SerializeObject(apiInput),
                                CreditStatus = 0
                            };

                            #endregion

                            try
                            {
                                //寫入刷卡狀態-未送
                                var sp_re = msp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

                                flag = mscom.Month_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);
                                if (WsOut != null)
                                    trace.traceAdd("CarTradeResult", new { WsOut });

                                if (flag && WsOut.ResponseParams != null)
                                {
                                    TransactionNo = WsOut.ResponseParams.ResultData.ServiceTradeNo;
                                    AuthCode = WsOut.ResponseParams.ResultData.AuthIdResp;
                                    CreditCardNo = WsOut.ResponseParams.ResultData.CardNumber;
                                    MerchantTradeNo = WsOut.ResponseParams.ResultData.MerchantTradeNo;
                                }

                                #region 更新刷卡狀態-一般

                                spIn.CreditStatus = flag ? 1 : 2;//刷卡結果0(未發送),1(回傳成功),2(回傳失敗),3(excetion)
                                if (WsOut != null)
                                    spIn.BankApiRe = JsonConvert.SerializeObject(WsOut);
                                //更新信用卡呼叫紀錄
                                var sp_re2 = msp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                errCode = "ERR270";
                                trace.BaseMsg = ex.Message;

                                #region 更新刷卡狀態-例外

                                spIn.CreditStatus = 3;//刷卡結果0(未發送),1(回傳成功),2(回傳失敗),3(excetion) 
                                spIn.Note = ex.Message;
                                var sp_re2 = msp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

                                #endregion

                                throw new Exception("TSIBTrade Fail");
                            }

                            trace.FlowList.Add("信用卡交易");
                        }

                        #endregion

                        #region 後續api處理

                        if (flag)
                        {
                            flag = buyNxtCom.exeNxt(MerchantTradeNo, TransactionNo);
                            errCode = buyNxtCom.errCode;
                            trace.traceAdd("exeNxt", new { buyNxtCom });
                            trace.FlowList.Add("後續api處理");
                        }
                        #endregion

                        #region 履保

                        if (flag)
                        {
                            try
                            {
                                mem = msp.GetMemberData(IDNO, LogID, Access_Token);
                                if (mem != null)
                                {
                                    var spin = new ICF_TSIB_Escrow_Type()
                                    {
                                        IDNO = IDNO,
                                        Name = mem.MEMCNAME,
                                        PhoneNo = mem.MEMTEL,
                                        Email = mem.MEMEMAIL,
                                        Amount = ProdPrice,
                                        UseType = 0,
                                        MonthlyNo = buyNxtCom.MonthlyRentId,
                                        PRGID = funName
                                    };
                                    var xFlag = mscom.TSIB_Escrow_Month(spin, ref errCode, ref errMsg);
                                    trace.traceAdd("TSIB_Escrow_Month", new { spin, xFlag, errCode, errMsg });
                                    trace.FlowList.Add("履保處理");
                                }
                            }
                            catch (Exception ex)
                            {
                                trace.traceAdd("Escrow_Exception", new { ex });
                            }

                        }

                        #endregion

                        #region 發票
                        if (flag)
                        {
                            try
                            {
                                WebAPIInput_MonthlyRentSave wsInput = new WebAPIInput_MonthlyRentSave()
                                {
                                    CUSTID = IDNO,
                                    CUSTNM = mem.MEMCNAME,
                                    EMAIL = mem.MEMEMAIL,
                                    MonRentID = buyNxtCom.MonthlyRentId,
                                    MonProjID = apiInput.UP_MonProjID,
                                    MonProPeriod = apiInput.UP_MonProPeriod,
                                    ShortDays = apiInput.UP_ShortDays,
                                    NowPeriod = buyNxtCom.NowPeriod,
                                    SDATE = buyNxtCom.OriSDATE.ToString("yyyyMMdd"),
                                    EDATE = buyNxtCom.OriSDATE.AddDays(apiInput.UP_MonProPeriod * 30).ToString("yyyyMMdd"),
                                    IsMoto = IsMoto,
                                    RCVAMT = ProdPrice,
                                    UNIMNO = InvData.UNIMNO,
                                    CARDNO = CreditCardNo,
                                    AUTHCODE = AuthCode,
                                    NORDNO = TransactionNo,
                                    INVKIND = InvData.InvocieType,
                                    CARRIERID = InvData.CARRIERID,
                                    NPOBAN = InvData.NPOBAN,
                                    INVTITLE = "",
                                    INVADDR = ""
                                };

                                wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>
                        {
                            new NPR130SavePaymentList()
                            {
                                PAYMENTTYPE = "1",
                                PAYTYPE = "4",
                                PAYAMT = ProdPrice,
                                PORDNO = TransactionNo,
                                PAYMEMO = "月租訂閱制"
                            }
                        };
                                WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();
                                var xflag = EasyRentApi.MonthlyRentSave(wsInput, ref wsOutput);
                                if (wsOutput.Result == false)
                                {
                                    xflag = false;
                                }
                                else
                                {
                                    INVNO = wsOutput.Data[0].INVNO;
                                    logger.Trace("INVNO=" + INVNO);
                                }
                                trace.traceAdd("MonthlyRentSave", new { wsInput, wsOutput });
                                trace.FlowList.Add("發票開立");
                                if (INVNO != "")
                                {
                                    //寫入發票資訊
                                    string sp_errCode = "";
                                    var spin = new SPInput_SaveInvno()
                                    {
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        NowPeriod = buyNxtCom.NowPeriod,
                                        PayTypeId = (Int64)apiInput.PayTypeId,
                                        InvoTypeId = InvoTypeId,
                                        InvoiceType = InvData.InvocieType,
                                        CARRIERID = InvData.CARRIERID,
                                        UNIMNO = InvData.UNIMNO,
                                        NPOBAN = InvData.NPOBAN,
                                        Invno = INVNO,
                                        InvoicePrice = ProdPrice,
                                        InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                        PRGID = funName,
                                        MonthlyRentID = buyNxtCom.MonthlyRentId
                                    };

                                    xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                                    if (!xflag)
                                    {
                                        logger.Trace("spError=" + sp_errCode);
                                    }
                                    trace.traceAdd("sp_SaveSubsInvno", new { spin, sp_errCode });
                                    trace.FlowList.Add("發票存檔");
                                }
                                else
                                {
                                    //20210826 ADD BY ADAM REASON.發票開立失敗處理
                                    //資料寫入錯誤紀錄log TB_MonthlyInvErrLog
                                    string sp_errCode = "";
                                    var spInput = new SPInput_InsMonthlyInvErr()
                                    {
                                        ApiInput = JsonConvert.SerializeObject(wsInput),
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        MonthlyRentID = buyNxtCom.MonthlyRentId,
                                        MonProjID = apiInput.UP_MonProjID,
                                        MonProPeriod = apiInput.UP_MonProPeriod,
                                        ShortDays = apiInput.UP_ShortDays,
                                        NowPeriod = buyNxtCom.NowPeriod,
                                        PayTypeId = (Int64)apiInput.PayTypeId,
                                        InvoTypeId = InvoTypeId,
                                        InvoiceType = InvData.InvocieType,
                                        CARRIERID = InvData.CARRIERID,
                                        UNIMNO = InvData.UNIMNO,
                                        NPOBAN = InvData.NPOBAN,
                                        INVAMT = ProdPrice,
                                        PRGID = funName,
                                        RtnCode = wsOutput?.RtnCode ?? "-4",
                                        RtnMsg = wsOutput?.Message ?? "",
                                    };

                                    xflag = msp.sp_InsMonthlyInvErr(spInput, ref sp_errCode);
                                    if (!xflag)
                                    {
                                        logger.Trace("spError=" + sp_errCode);
                                    }
                                    trace.traceAdd("sp_InsMonthlyInvErr", new { spInput, sp_errCode });
                                    trace.FlowList.Add("發票錯誤處理");
                                }
                            }
                            catch (Exception ex)
                            {
                                //紀錄開立失敗
                                logger.Trace(ex.Message);
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #endregion

                outputApi.PayResult = flag ? 1 : 0;

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(250, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
        #endregion

    }

    public class IAPI_BuyNowTool_AddMonth : IAPI_BuyNow_AddMonth
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransactionNo { get; set; }
        /// <summary>
        /// 合約金額
        /// </summary>
        public int ProdPrice { get; set; }
        /// <summary>
        /// 訂閱制識別ID
        /// </summary>
        public int MonthlyRentId { get; set; }
        /// <summary>
        /// 是否付費(0:否 1是)
        /// </summary>
        public int CreditCardFlg { get; set; }
        /// <summary>
        /// 是否訂閱(0:否 1是)
        /// </summary>
        public int SubsDataFlg { get; set; }
        /// <summary>
        /// 是否履保(0:否 1是)
        /// </summary>
        public int EscrowMonthFlg { get; set; }
        /// <summary>
        /// 是否開發票(0:否 1是)
        /// </summary>
        public int InvoiceFlg { get; set; }
    }

    public class IAPI_BuyNowTool_DoUpMonth : IAPI_BuyNow_AddMonth
    {
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 升轉專案編號
        /// </summary>
        public string UP_MonProjID { get; set; } = "";
        /// <summary>
        /// 升轉期數
        /// </summary>
        public int UP_MonProPeriod { get; set; } = 0;
        /// <summary>
        /// 升轉短天期
        /// </summary>
        public int UP_ShortDays { get; set; } = 0;
        /// <summary>
        /// 是否付費(0:否 1是)
        /// </summary>
        public int CreditCardFlg { get; set; }
        /// <summary>
        /// 商店訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; }
        /// <summary>
        /// 台新訂單編號
        /// </summary>
        public string TransactionNo { get; set; }

    }
}
