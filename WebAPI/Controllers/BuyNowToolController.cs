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
            int IsMoto = 0;
            string TransactionNo = "";
            string CreditCardNo = "";
            string AuthCode = "";
            string MerchantTradeNo = "";
            string INVNO = "";
            int MonthlyRentId = 0;
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
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) ||
                           apiInput.MonProPeriod == 0 
                           )
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                    }

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }


                    MerchantTradeNo = apiInput.MerchantTradeNo;
                    TransactionNo = apiInput.TransactionNo;
                    MonthlyRentId = apiInput.MonthlyRentId;
                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    buyNxtCom.IDNO = IDNO;
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB
                if (flag)
                {
                    #region 載入後續Api所需資料

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
                        var monObjs = msp.sp_GetMonSetInfo(spin, ref sp_errCode);
                        if (monObjs != null && monObjs.Count() > 0)
                        {
                            var fItem = monObjs.FirstOrDefault();
                            if (fItem.PeriodPrice > 0)
                                ProdPrice = fItem.PeriodPrice;

                            IsMoto = fItem.IsMoto;
                        }
                        var nowPeriodMonData = msp.Sql_GetMonData(apiInput.MonthlyRentId);
                        SDate = Convert.ToDateTime(nowPeriodMonData["StartDate"]).ToString("yyyyMMdd");
                        EDate = Convert.ToDateTime(nowPeriodMonData["EndDate"]).ToString("yyyyMMdd");
                        nowPeriod = Convert.ToInt32(nowPeriodMonData["NowPeriod"]);
                        trace.traceAdd("monthInfo", new { monObjs, spin, sp_errCode });
                        trace.FlowList.Add("月租資訊");
                    }

                    #endregion

                    #region 信用卡交易

                    var WsOut = new WebAPIOutput_Auth();
                    if (apiInput.CreditCardFlg == 1 && ProdPrice > 0) //有價格才進行信用卡交易
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
                    if (apiInput.InvoiceFlg==1 && flag)
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
                                NowPeriod = nowPeriod,
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
                            trace.FlowList.Add("發票開立");
                            if (INVNO != "")
                            {
                                //寫入發票資訊
                                string sp_errCode = "";
                                var spin = new SPInput_SaveInvno()
                                {
                                    IDNO = IDNO,
                                    LogID = LogID,
                                    MonProjID = apiInput.MonProjID,
                                    MonProPeriod = apiInput.MonProPeriod,
                                    ShortDays = apiInput.ShortDays,
                                    NowPeriod = nowPeriod,
                                    PayTypeId = (Int64)apiInput.PayTypeId,
                                    InvoTypeId = InvoTypeId,
                                    InvoiceType = InvData.InvocieType,
                                    CARRIERID = InvData.CARRIERID,
                                    UNIMNO = InvData.UNIMNO,
                                    NPOBAN = InvData.NPOBAN,
                                    Invno = INVNO,
                                    InvoicePrice = ProdPrice,
                                    InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                    PRGID=funName
                                };

                                xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                                if (!xflag)
                                {
                                    logger.Trace("spError=" + sp_errCode);
                                }
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
                                    MonthlyRentID = apiInput.MonthlyRentId == 0 ? buyNxtCom.MonthlyRentId : apiInput.MonthlyRentId,
                                    MonProjID = apiInput.MonProjID,
                                    MonProPeriod = apiInput.MonProPeriod,
                                    ShortDays = apiInput.ShortDays,
                                    NowPeriod = nowPeriod,
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
                    
                    outputApi.PayResult = flag ? 1 : 0;
                }

                #endregion

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(181, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

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
            string funName = "BuyNow_DoUpMonth";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNow_UpMonth();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNow_UpMonth>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.MonProjID) ||
                           apiInput.MonProPeriod == 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                    }

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }

                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    buyNxtCom.IDNO = IDNO;
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB

                if (flag)
                {
                    #region 載入後續Api所需資料

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

                    #region 信用卡交易

                    var WsOut = new WebAPIOutput_Auth();
                    if (false && ProdPrice > 0) //有價格才進行信用卡交易
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

                    if (false && flag)
                    {
                        flag = buyNxtCom.exeNxt(MerchantTradeNo, TransactionNo);
                        errCode = buyNxtCom.errCode;
                        trace.traceAdd("exeNxt", new { buyNxtCom });
                        trace.FlowList.Add("後續api處理");
                    }
                    outputApi.PayResult = flag ? 1 : 0;

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
                                    UseType=0,
                                    MonthlyNo= buyNxtCom.MonthlyRentId,
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
                            NowPeriod = buyNxtCom.NowPeriod, //第一期固定寫1
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
                        trace.FlowList.Add("發票開立");
                        if (INVNO != "")
                        {
                            //寫入發票資訊
                            string sp_errCode = "";
                            var spin = new SPInput_SaveInvno()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
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
                                Invno = INVNO,
                                InvoicePrice = ProdPrice,
                                InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                PRGID=funName
                            };

                            xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                            if (!xflag)
                            {
                                logger.Trace("spError=" + sp_errCode);
                            }
                            trace.FlowList.Add("發票存檔");
                        }
                    }
                    catch (Exception ex)
                    {
                        //紀錄開立失敗
                        logger.Trace(ex.Message);
                    }



                    #endregion

                }

                #endregion

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(181, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

        [Route("api/BuyNowTool/DoPayArrs")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoPayArrs([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon
            {
                ApiID = 190
            };
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNow_DoPayArrs";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNow_PayArrs();
            var outputApi = new OAPI_BuyNow_Base();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            int ProdPrice = 0;

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

            if (value == null)
                value = new Dictionary<string, object>();

            try
            {
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNow_PayArrs>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }

                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    buyNxtCom.IDNO = IDNO;
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB

                if (flag)
                {
                    #region 載入後續Api所需資料

                    string MonthlyRentIds = "";
                    if (apiInput.MonthlyRentIds != null && apiInput.MonthlyRentIds.Count() > 0)
                    {
                        MonthlyRentIds = String.Join(",", apiInput.MonthlyRentIds);
                    }

                    //從資料庫找發票設定 InvoTypeId
                    InvData = mscom.GetINVDataFromMember(IDNO);
                    Int64 InvoTypeId = InvData.InvocieTypeId;
                    buyNxtCom.LogID = LogID;
                    var objJson = new
                    {
                        MonthlyRentIds
                    };
                    buyNxtCom.ApiJson = JsonConvert.SerializeObject(objJson);
                    buyNxtCom.PayTypeId = 5;//TB_CodeId=5目前只有信用卡付款
                    buyNxtCom.InvoTypeId = InvoTypeId;
                    flag = buyNxtCom.CkApiID();
                    errCode = buyNxtCom.errCode;

                    #endregion

                    #region 取得欠費總額
                    var sp_re = new SPOut_GetArrsSubsList();
                    if (flag)
                    {
                        string sp_errCode = "";
                        var spin = new SPInput_GetArrsSubsList()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            //SetNow = apiInput.SetNow
                        };
                        sp_re = msp.sp_GetArrsSubsList(spin, ref sp_errCode);
                        if (sp_re != null && sp_re.Arrs != null && sp_re.Arrs.Count() > 0)
                        {
                            var allArrs = sp_re.Arrs.Where(x => x.PeriodPayPrice > 0).Select(y => y.PeriodPayPrice).Sum();
                            ProdPrice = allArrs;
                            //20210717 ADD BY ADAM REASON.
                            MonthlyRentIds = String.Join(",", sp_re.Arrs.Select(x => x.MonthlyRentId));
                            buyNxtCom.MonthlyRentIds = MonthlyRentIds;
                        }
                    }

                    #endregion

                    #region 信用卡交易

                    var WsOut = new WebAPIOutput_Auth();
                    if (false && ProdPrice > 0) //有價格才進行信用卡交易
                    {
                        trace.traceAdd("CarTradeIn", new { IDNO, ProdPrice, errCode });
                        try
                        {
                            flag = mscom.MonArrears_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);
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

                    if (flag)
                    {
                        flag = buyNxtCom.exeNxt(MerchantTradeNo, TransactionNo);
                        errCode = buyNxtCom.errCode;
                        trace.FlowList.Add("後續api處理");
                    }
                    outputApi.PayResult = flag ? 1 : 0;

                    #endregion

                    #region 履保

                    if (flag)
                    {
                        try
                        {
                            mem = msp.GetMemberData(IDNO, LogID, Access_Token);
                            if (mem != null)
                            {
                                for (int i = 0; i < sp_re.Arrs.Count; i++)
                                {
                                    var spin = new ICF_TSIB_Escrow_Type()
                                    {
                                        IDNO = IDNO,
                                        Name = mem.MEMCNAME,
                                        PhoneNo = mem.MEMTEL,
                                        Email = mem.MEMEMAIL,
                                        Amount = sp_re.Arrs[i].PeriodPayPrice,
                                        UseType = 0,
                                        MonthlyNo = sp_re.Arrs[i].MonthlyRentId
                                    };
                                    var xFlag = mscom.TSIB_Escrow_Month(spin, ref errCode, ref errMsg);
                                    trace.traceAdd("TSIB_Escrow_Month", new { spin, xFlag, errCode, errMsg });                                   
                                }
                                trace.FlowList.Add("履保處理");

                            }
                        }
                        catch (Exception ex)
                        {
                            trace.traceAdd("Escrow_Exception", new { ex });
                        }
                    }

                    #endregion

                    if (flag)
                    {
                        try
                        {
                            WebAPIInput_MonthlyRebtSaveV2 wsInput = new WebAPIInput_MonthlyRebtSaveV2()
                            {
                                CUSTID = IDNO,
                                CUSTNM = mem.MEMCNAME,
                                EMAIL = mem.MEMEMAIL,
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
                            wsInput.tbMonthlyRentProjData = new List<WebAPIInput_MonthlyRentProjData>();
                            for (int i = 0; i < sp_re.Arrs.Count; i++)
                            {
                                wsInput.tbMonthlyRentProjData.Add(new WebAPIInput_MonthlyRentProjData()
                                {
                                    MonRentID = sp_re.Arrs[i].MonthlyRentId,
                                    MonProjID = sp_re.Arrs[i].ProjID,
                                    MonProPeriod = sp_re.Arrs[i].MonProPeriod,
                                    ShortDays = sp_re.Arrs[i].ShortDays,
                                    NowPeriod = sp_re.Arrs[i].rw,
                                    SDATE = sp_re.Arrs[i].StartDate.ToString("yyyyMMdd"),
                                    EDATE = sp_re.Arrs[i].EndDate.ToString("yyyyMMdd"),
                                    IsMoto = sp_re.Arrs[i].IsMoto
                                });
                            }

                            WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();
                            var xflag = EasyRentApi.MonthlyRentSaveV2(wsInput, ref wsOutput);
                            if (wsOutput.Result == false)
                            {
                                xflag = false;
                            }
                            else
                            {
                                INVNO = wsOutput.Data[0].INVNO;
                                logger.Trace("INVNO=" + INVNO);
                            }
                            trace.FlowList.Add("發票開立");
                            if (INVNO != "")
                            {
                                //寫入發票資訊
                                for (int i = 0; i < sp_re.Arrs.Count; i++)
                                {
                                    string sp_errCode = "";
                                    var spin = new SPInput_SaveInvno()
                                    {
                                        IDNO = IDNO,
                                        LogID = LogID,
                                        MonProjID = sp_re.Arrs[i].ProjID,
                                        MonProPeriod = sp_re.Arrs[i].MonProPeriod,
                                        ShortDays = sp_re.Arrs[i].ShortDays,
                                        NowPeriod = sp_re.Arrs[i].rw,
                                        PayTypeId = (Int64)apiInput.PayTypeId,
                                        InvoTypeId = InvoTypeId,
                                        InvoiceType = InvData.InvocieType,
                                        CARRIERID = InvData.CARRIERID,
                                        UNIMNO = InvData.UNIMNO,
                                        NPOBAN = InvData.NPOBAN,
                                        Invno = INVNO,
                                        InvoicePrice =ProdPrice,
                                        InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                        PRGID=funName
                                    };

                                    xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                                    if (!xflag)
                                    {
                                        logger.Trace("spError=" + sp_errCode);
                                    }
                                }
                                trace.FlowList.Add("發票存檔");
                            }
                        }
                        catch (Exception ex)
                        {
                            //紀錄開立失敗
                            logger.Trace(ex.Message);
                        }
                    }
                }

                #endregion

                trace.traceAdd("outputApi", outputApi);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            carRepo.AddTraceLog(181, funName, trace, flag);

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }

    public class IAPI_BuyNowTool_AddMonth : IAPI_BuyNow_AddMonth
    {
        public string MerchantTradeNo { get; set; }
        public string TransactionNo { get; set; }
        public int MonthlyRentId { get; set; }
        public int CreditCardFlg { get; set; }
        public int SubsDataFlg { get; set; }
        public int EscrowMonthFlg { get; set; }
        public int InvoiceFlg { get; set; }
    }
}
