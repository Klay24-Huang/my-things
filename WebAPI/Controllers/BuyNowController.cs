using Domain.Common;
using Domain.SP.Input.Subscription;
using Domain.SP.Output.Subscription;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 立即購買
    /// </summary>
    public class BuyNowController : ApiController
    {
        private List<Int64> payList = new List<Int64>() { 0 };
        private List<Int64> invoList = new List<Int64>() { 1, 2, 3, 4, 5, 6 };
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        #region ori

        //[Route("api/BuyNow/BuyAny")]
        //[HttpPost()]
        //public async Task<Dictionary<string, object>> DoBuyNow([FromBody] Dictionary<string, object> value)
        //{
        //    #region 初始宣告
        //    var mscom = new MonSubsCommon();
        //    var msp = new MonSubsSp();
        //    var buyNxtCom = new BuyNowNxtCommon();
        //    var cr_com = new CarRentCommon();
        //    var trace = new TraceCom();
        //    var carRepo = new CarRentRepo();
        //    HttpContext httpContext = HttpContext.Current;
        //    //string[] headers=httpContext.Request.Headers.AllKeys;
        //    string Access_Token = "";
        //    string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
        //    var objOutput = new Dictionary<string, object>();    //輸出
        //    bool flag = true;
        //    string errMsg = "Success"; //預設成功
        //    string errCode = "000000"; //預設成功
        //    string funName = "BuyNowController";
        //    Int64 LogID = 0;
        //    var apiInput = new IAPI_BuyNow();
        //    var outputApi = new OAPI_BuyNow();
        //    outputApi.PayTypes = new List<OPAI_TypeListParam>();
        //    outputApi.InvoTypes = new List<OPAI_TypeListParam>();
        //    Token token = null;
        //    CommonFunc baseVerify = new CommonFunc();
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();

        //    string Contentjson = "";
        //    bool isGuest = true;
        //    string IDNO = "";
        //    int ProdPrice = 0;

        //    #endregion

        //    trace.traceAdd("apiIn", value);

        //    try
        //    {
        //        #region 防呆

        //        flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
        //        if (flag)
        //        {
        //            apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNow>(Contentjson);
        //            //寫入API Log
        //            string ClientIP = baseVerify.GetClientIp(Request);
        //            flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

        //            //不開放訪客
        //            if (flag)
        //            {
        //                if (isGuest)
        //                {
        //                    flag = false;
        //                    errCode = "ERR101";
        //                }
        //            }
        //            if (flag)
        //            {
        //                var DoPays = new List<int> { 0, 1 };
        //                if (!DoPays.Any(x=>x == apiInput.DoPay))
        //                {
        //                    flag = false;
        //                    errCode = "ERR266";//DoPay只可為0或1
        //                }
        //            }

        //            if (flag)
        //            {
        //                if(apiInput.DoPay == 0)
        //                {
        //                    if (apiInput.ApiID == 190)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        if (string.IsNullOrWhiteSpace(apiInput.ProdNm))
        //                        {
        //                            flag = false;
        //                            errCode = "ERR269";
        //                        }
        //                    }
        //                }                     
        //            }

        //            //載入後續Api所需資料
        //            if (flag && apiInput.ApiID > 0)
        //            {
        //                buyNxtCom.LogID = LogID;
        //                buyNxtCom.ApiID = apiInput.ApiID;
        //                buyNxtCom.ApiJson = apiInput.ApiJson;
        //                buyNxtCom.PayTypeId = apiInput.PayTypeId;
        //                buyNxtCom.InvoTypeId = apiInput.InvoTypeId;
        //                flag = buyNxtCom.CkApiID();
        //                errCode = buyNxtCom.errCode;
        //            }

        //            if (flag)
        //            {
        //                if(apiInput.DoPay == 1)
        //                {
        //                    if (apiInput.PayTypeId == 0 || apiInput.InvoTypeId == 0)
        //                    {
        //                        flag = false;
        //                        errCode = "ERR268";
        //                    }                            
        //                }
        //            }

        //            if (flag)
        //            {
        //                if (apiInput.ProdPrice > 0)
        //                    ProdPrice = apiInput.ProdPrice;
        //            }

        //            trace.FlowList.Add("防呆");
        //        }

        //        #endregion

        //        #region Token判斷

        //        if (flag && isGuest == false)
        //        {
        //            var token_in = new IBIZ_TokenCk
        //            {
        //                LogID = LogID,
        //                Access_Token = Access_Token
        //            };
        //            var token_re = cr_com.TokenCk(token_in);
        //            if (token_re != null)
        //            {
        //                trace.traceAdd(nameof(token_re), token_re);
        //                flag = token_re.flag;
        //                errCode = token_re.errCode;
        //                lstError = token_re.lstError;
        //                IDNO = token_re.IDNO;

        //                buyNxtCom.IDNO = IDNO;                      
        //            }
        //            trace.FlowList.Add("Token判斷");
        //        }

        //        #endregion

        //        #region TB

        //        if (flag)
        //        {
        //            if (apiInput.DoPay == 0)
        //            {
        //                var spIn = new SPInput_GetBuyNowInfo()
        //                {
        //                    IDNO = IDNO,
        //                    LogID = LogID
        //                };
        //                trace.traceAdd("spIn", spIn);
        //                var spList = msp.sp_GetBuyNowInfo(spIn, ref errCode);

        //                if (spList != null && spList.Count() > 0)
        //                {
        //                    trace.traceAdd("spList", spList);

        //                    var payTypes = spList.Where(x => x.CodeGroup == "PayType").OrderBy(y=>y.Sort).ToList();
        //                    var invoTypes = spList.Where(x => x.CodeGroup == "InvoiceType").OrderBy(y=>y.Sort).ToList();

        //                    if (payTypes != null && payTypes.Count() > 0)
        //                    {
        //                        outputApi.PayTypes = (from a in payTypes
        //                                              select new OPAI_TypeListParam
        //                                              {
        //                                                  CodeId = Convert.ToInt32(a.CodeId),
        //                                                  CodeNm = a.CodeNm,
        //                                                  IsBind = a.IsBind
        //                                              }).ToList();
        //                    }

        //                    if (invoTypes != null && invoTypes.Count() > 0)
        //                    {
        //                        outputApi.InvoTypes = (from a in invoTypes
        //                                               select new OPAI_TypeListParam
        //                                               {
        //                                                   CodeId = Convert.ToInt32(a.CodeId),
        //                                                   CodeNm = a.CodeNm,
        //                                                   IsBind = a.IsBind
        //                                               }).ToList();
        //                    }


        //                    if(apiInput.ApiID != 190)
        //                    {
        //                        outputApi.ProdNm = apiInput.ProdNm;
        //                        outputApi.ProdDisc = apiInput.ProdDisc;
        //                    }

        //                    outputApi.ProdPrice = apiInput.ProdPrice;
        //                }
        //            }
        //            else if (apiInput.DoPay == 1)//付款
        //            {
        //                #region 修改預設付款方式

        //                if(!string.IsNullOrWhiteSpace(IDNO) &&
        //                   LogID >0 && apiInput.PayTypeId >0 && apiInput.InvoTypeId > 0)
        //                {
        //                    var xspin = new SPInput_SetSubsPayInvoDef()
        //                    {
        //                        IDNO = IDNO,
        //                        LogID = LogID,
        //                        InvoTypeId = apiInput.InvoTypeId,
        //                        PayTypeId = apiInput.PayTypeId
        //                    };
        //                    msp.sp_SubsPayInvoDef(xspin, ref errCode);
        //                }

        //                #endregion

        //                #region 信用卡交易

        //                var WsOut = new WebAPIOutput_Auth();
        //                if (ProdPrice > 0) //有價格才進行信用卡交易
        //                {
        //                    trace.traceAdd("CarTradeIn", new { IDNO, ProdPrice, errCode });
        //                    try
        //                    {
        //                        if(apiInput.ApiID == 190)//月租欠費
        //                            flag = mscom.MonArrears_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);
        //                        else
        //                            flag = mscom.Month_TSIBTrade(IDNO, ref WsOut, ref ProdPrice, ref errCode);

        //                        if (WsOut != null)
        //                            trace.traceAdd("CarTradeResult", new { WsOut });
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        flag = false;
        //                        errCode = "ERR270";
        //                        trace.BaseMsg = ex.Message;
        //                        throw new Exception("TSIBTrade Fail");
        //                    }

        //                    trace.FlowList.Add("信用卡交易");
        //                }

        //                #endregion

        //                if (flag)
        //                {           
        //                    if(apiInput.ApiID > 0)
        //                    {
        //                        flag = buyNxtCom.exeNxt();
        //                        errCode = buyNxtCom.errCode;
        //                        trace.FlowList.Add("後續api處理");
        //                    }
        //                }
        //                outputApi.PayResult = flag ? 1 : 0;
        //            }               
        //        }

        //        #endregion

        //        trace.traceAdd("outputApi", outputApi);
        //    }
        //    catch (Exception ex)
        //    {
        //        trace.BaseMsg = ex.Message;
        //    }            

        //    carRepo.AddTraceLog(181, funName, trace, flag);

        //    #region 輸出
        //    baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
        //    return objOutput;
        //    #endregion        
        //}

        #endregion

        #region 購買月租
        [Route("api/BuyNow/DoAddMonth")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoAddMonth([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon();
            buyNxtCom.ApiID = 179;
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNow_DoAddMonthController";
            Int64 LogID = 0;
            var apiInput = new IAPI_BuyNow_AddMonth();
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
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BuyNow_AddMonth>(Contentjson);
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

                    //20210617 ADD BY ADAM REASON.發票設定改為從內部取
                    /*
                    if (flag)
                    {
                        if (!payList.Any(x => x == apiInput.PayTypeId) || !invoList.Any(y => y == apiInput.InvoTypeId))
                        {
                            flag = false;
                            errCode = "ERR268";
                        }
                    }*/

                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;

                        buyNxtCom.IDNO = IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB

                //是否已刷過卡
                if (flag)
                {//24小時內相同呼叫條件視為相同
                    string spErrCode = "";
                    var spIn = new SPInput_GetSubsCreditStatus()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        APIID = 181,
                        ActionNM = funName,
                        ApiCallKey = JsonConvert.SerializeObject(apiInput)
                    };
                    var sp_list = msp.sp_GetSubsCreditStatus(spIn, ref spErrCode);
                    if (sp_list != null && sp_list.Count() > 0)
                    {
                        flag = false;
                        errCode = "ERR277";
                        errMsg = "刷卡已存在";
                    }
                }

                //先檢查是否可以購買訂閱制
                //目前兩個情況會擋掉，積分小於60，已經有重複買的也不能
                if (flag)
                {
                    string spErrCode = "";
                    var spIn = new SPInput_BuyNowAddMonth_Q01()
                    {
                        IDNO = IDNO,
                        MonProjId = apiInput.MonProjID,
                        MonProPeroid = apiInput.MonProPeriod,
                        ShortDays = apiInput.ShortDays,
                        LogID = LogID
                    };
                    if (!msp.sp_BuyNowAddMonth_Q01(spIn, ref spErrCode))
                    {
                        flag = false;
                        errCode = spErrCode;
                        errMsg = "購買失敗!";
                    }
                }

                if (flag)
                {
                    #region 載入後續Api所需資料

                    //Int64 InvoTypeId = mscom.GetInvoCodeId(Convert.ToInt32(apiInput.InvoTypeId));
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

                        trace.traceAdd("monthInfo", new { monObjs, spin, sp_errCode });
                        trace.FlowList.Add("月租資訊");
                    }

                    #endregion

                    #region 信用卡交易

                    var WsOut = new WebAPIOutput_Auth();
                    if (ProdPrice > 0) //有價格才進行信用卡交易
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
                        trace.FlowList.Add("後續api處理");
                    }
                    #endregion

                    #region 履保
                    if (flag)
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
                                    Amount = ProdPrice
                                };
                                var xFlag = mscom.TSIB_Escrow_Month(spin, ref xerrCode, ref xerrMsg);
                                trace.traceAdd("Contract", new { spin, xFlag, xerrCode, xerrMsg });
                                trace.FlowList.Add("履保處理");
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    #endregion

                    #region 發票
                    if (flag && ProdPrice > 0)
                    {
                        try
                        {
                            WebAPIInput_MonthlyRentSave wsInput = new WebAPIInput_MonthlyRentSave()
                            {
                                CUSTID = IDNO,
                                CUSTNM = mem.MEMCNAME,
                                EMAIL = mem.MEMEMAIL,
                                MonRentID = buyNxtCom.MonthlyRentId,
                                MonProjID = apiInput.MonProjID,
                                MonProPeriod = apiInput.MonProPeriod,
                                ShortDays = apiInput.ShortDays,
                                NowPeriod = 1, //第一期固定寫1
                                SDATE = DateTime.Now.ToString("yyyyMMdd"),
                                EDATE = DateTime.Now.AddDays(apiInput.MonProPeriod * 30).ToString("yyyyMMdd"),
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

                            wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>();
                            wsInput.tbPaymentDetail.Add(new NPR130SavePaymentList()
                            {
                                PAYMENTTYPE = "1",
                                PAYTYPE = "4",
                                PAYAMT = ProdPrice,
                                PORDNO = TransactionNo,
                                PAYMEMO = "月租訂閱制"
                            });
                            WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();
                            var xflag = EasyRentApi.MonthlyRentSave(wsInput, ref wsOutput);
                            if (wsOutput.Result == false)
                            {
                                xflag = false;
                                logger.Trace("發票開立失敗!MonthlyRentId=" + wsInput.MonRentID.ToString());
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
                                    NowPeriod = 1,  //寫死第一期
                                    PayTypeId = (Int64)apiInput.PayTypeId,
                                    InvoTypeId = InvoTypeId,
                                    InvoiceType = InvData.InvocieType,
                                    CARRIERID = InvData.CARRIERID,
                                    UNIMNO = InvData.UNIMNO,
                                    NPOBAN = InvData.NPOBAN,
                                    Invno = INVNO,
                                    InvoicePrice = ProdPrice,
                                    InvoiceDate = DateTime.Now.ToString("yyyyMMdd")
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
                                    MonthlyRentID = buyNxtCom.MonthlyRentId,
                                    MonProjID = apiInput.MonProjID,
                                    MonProPeriod = apiInput.MonProPeriod,
                                    ShortDays = apiInput.ShortDays,
                                    NowPeriod = 1,
                                    PayTypeId = (Int64)apiInput.PayTypeId,
                                    InvoTypeId = InvoTypeId,
                                    InvoiceType = InvData.InvocieType,
                                    CARRIERID = InvData.CARRIERID,
                                    UNIMNO = InvData.UNIMNO,
                                    NPOBAN = InvData.NPOBAN,
                                    INVAMT = ProdPrice
                                };

                                xflag = msp.sp_InsMonthlyInvErr(spInput, ref sp_errCode);
                                if (!xflag)
                                {
                                    logger.Trace("spError=" + sp_errCode);
                                }
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
        #endregion

        #region 月租升轉
        [Route("api/BuyNow/DoUpMonth")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoUpMonth([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon();
            buyNxtCom.ApiID = 188;
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNow_DoUpMonthController";
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
                    //20210617 ADD BY ADAM REASON.發票設定改為從內部取
                    /*
                    if (flag)
                    {
                        if (!payList.Any(x => x == apiInput.PayTypeId) || !invoList.Any(y => y == apiInput.InvoTypeId))
                        {
                            flag = false;
                            errCode = "ERR268";
                        }
                    }*/

                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;

                        buyNxtCom.IDNO = IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB

                //是否已刷過卡
                if (flag)
                {//24小時內相同呼叫條件視為相同
                    string spErrCode = "";
                    var spIn = new SPInput_GetSubsCreditStatus()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        APIID = 181,
                        ActionNM = funName,
                        ApiCallKey = JsonConvert.SerializeObject(apiInput)
                    };
                    var sp_list = msp.sp_GetSubsCreditStatus(spIn, ref spErrCode);
                    if (sp_list != null && sp_list.Count() > 0)
                    {
                        flag = false;
                        errCode = "ERR277";
                        errMsg = "刷卡已存在";
                    }
                }

                if (flag)
                {
                    #region 載入後續Api所需資料

                    //Int64 InvoTypeId = mscom.GetInvoCodeId(Convert.ToInt32(apiInput.InvoTypeId));
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
                    if (ProdPrice > 0) //有價格才進行信用卡交易
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
                                    Amount = ProdPrice
                                };
                                var xFlag = mscom.TSIB_Escrow_Month(spin, ref errCode, ref errMsg);
                            }
                        }
                        catch (Exception ex)
                        {

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

                        wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>();
                        wsInput.tbPaymentDetail.Add(new NPR130SavePaymentList()
                        {
                            PAYMENTTYPE = "1",
                            PAYTYPE = "4",
                            PAYAMT = ProdPrice,
                            PORDNO = TransactionNo,
                            PAYMEMO = "月租訂閱制"
                        });
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
                                InvoiceDate = DateTime.Now.ToString("yyyyMMdd")
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
        #endregion

        #region 月租欠費
        [Route("api/BuyNow/DoPayArrs")]
        [HttpPost()]
        public async Task<Dictionary<string, object>> DoPayArrs([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var mscom = new MonSubsCommon();
            var msp = new MonSubsSp();
            var buyNxtCom = new BuyNowNxtCommon();
            buyNxtCom.ApiID = 190;
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BuyNow_DoPayArrsController";
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

                    //20210617 ADD BY ADAM REASON.發票設定改為從內部取
                    /*
                    if (flag)
                    {
                        
                        if (!payList.Any(x => x == apiInput.PayTypeId) || !invoList.Any(y => y == apiInput.InvoTypeId))
                        {
                            flag = false;
                            errCode = "ERR268";
                        }
                    }*/

                    trace.FlowList.Add("防呆");
                }

                #endregion

                #region Token判斷

                if (flag && isGuest == false)
                {
                    var token_in = new IBIZ_TokenCk
                    {
                        LogID = LogID,
                        Access_Token = Access_Token
                    };
                    var token_re = cr_com.TokenCk(token_in);
                    if (token_re != null)
                    {
                        trace.traceAdd(nameof(token_re), token_re);
                        flag = token_re.flag;
                        errCode = token_re.errCode;
                        lstError = token_re.lstError;
                        IDNO = token_re.IDNO;

                        buyNxtCom.IDNO = IDNO;
                    }
                    trace.FlowList.Add("Token判斷");
                }

                #endregion

                #region TB

                //是否已刷過卡
                if (flag)
                {//24小時內相同呼叫條件視為相同
                    string spErrCode = "";
                    var spIn = new SPInput_GetSubsCreditStatus()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        APIID = 181,
                        ActionNM = funName,
                        ApiCallKey = JsonConvert.SerializeObject(apiInput)
                    };
                    var sp_list = msp.sp_GetSubsCreditStatus(spIn, ref spErrCode);
                    if (sp_list != null && sp_list.Count() > 0)
                    {
                        flag = false;
                        errCode = "ERR277";
                        errMsg = "刷卡已存在";
                    }
                }

                if (flag)
                {
                    #region 載入後續Api所需資料

                    string MonthlyRentIds = "";
                    if (apiInput.MonthlyRentIds != null && apiInput.MonthlyRentIds.Count() > 0)
                    {
                        MonthlyRentIds = String.Join(",", apiInput.MonthlyRentIds);
                    }

                    //從資料庫找發票設定 InvoTypeId
                    //Int64 InvoTypeId = mscom.GetInvoCodeId(Convert.ToInt32(apiInput.InvoTypeId));
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
                    if (ProdPrice > 0) //有價格才進行信用卡交易
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
                            var xsp_re = msp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

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
                                    Amount = ProdPrice
                                };
                                var xFlag = mscom.TSIB_Escrow_Month(spin, ref errCode, ref errMsg);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion

                    if (flag && ProdPrice > 0)  //阻擋0元發票
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

                            wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>();
                            wsInput.tbPaymentDetail.Add(new NPR130SavePaymentList()
                            {
                                PAYMENTTYPE = "1",
                                PAYTYPE = "4",
                                PAYAMT = ProdPrice,
                                PORDNO = TransactionNo,
                                PAYMEMO = "月租訂閱制"
                            });
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
                                        InvoicePrice = ProdPrice,
                                        InvoiceDate = DateTime.Now.ToString("yyyyMMdd")
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
        #endregion
    }
}