using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input;
using Domain.SP.Input.Member;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Subscription;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 訂閱制自動續訂
    /// </summary>
    public class AutoSubscribeJobController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoAutoSubscribeJob(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];    //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "AutoSubscribeJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            NullOutput outputApi = new NullOutput();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;

            List<SPOut_AutoSubscribeJob> SPOutList = new List<SPOut_AutoSubscribeJob>();
            RegisterData MemberData = new RegisterData();

            var MonSubsSp = new MonSubsSp();
            var MonSubsCommon = new MonSubsCommon();
            var NewMonthlyRentID = 0;

            HiEasyRentAPI EasyRentApi = new HiEasyRentAPI();
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("No Input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB
            #region 取得續訂清單
            if (flag)
            {
                string spName = "usp_AutoSubscribeJob_Q01";
                SPInput_Base spInput = new SPInput_Base
                {
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_Base, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_Base, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref SPOutList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion
            #region 自動續訂
            if (flag && SPOutList != null && SPOutList.Count > 0)
            {
                logger.Trace("AutoSubscribeJob Count:" + SPOutList.Count.ToString());

                foreach (var item in SPOutList)
                {
                    var PeriodPayPrice = item.PeriodPayPrice;
                    string TransactionNo = "";
                    string AuthCode = "";
                    string CreditCardNo = "";
                    string MerchantTradeNo = "";

                    #region 信用卡交易
                    if (PeriodPayPrice > 0)
                    {
                        string spErrCode = "";
                        var spIn = new SPInput_SetSubsCreditStatus()
                        {
                            IDNO = item.IDNO,
                            LogID = LogID,
                            APIID = 0,
                            ActionNM = funName,
                            ApiCallKey = JsonConvert.SerializeObject(item),
                            CreditStatus = 0    // 0:未發送
                        };

                        try
                        {
                            var WsOut = new WebAPIOutput_Auth();

                            //寫入刷卡狀態-未送
                            var sp_re = MonSubsSp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

                            flag = MonSubsCommon.Month_TSIBTrade(item.IDNO, ref WsOut, ref PeriodPayPrice, ref errCode);

                            if (flag && WsOut.ResponseParams != null)
                            {
                                TransactionNo = WsOut.ResponseParams.ResultData.ServiceTradeNo;
                                AuthCode = WsOut.ResponseParams.ResultData.AuthIdResp;
                                CreditCardNo = WsOut.ResponseParams.ResultData.CardNumber;
                                MerchantTradeNo = WsOut.ResponseParams.ResultData.MerchantTradeNo;
                            }

                            spIn.CreditStatus = flag ? 1 : 2;   // 1:回傳成功,2:回傳失敗
                            if (WsOut != null)
                            {
                                spIn.BankApiRe = JsonConvert.SerializeObject(WsOut);
                            }
                            //更新信用卡呼叫紀錄
                            var sp_re2 = MonSubsSp.sp_SetSubsCreditStatus(spIn, ref spErrCode);
                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            errCode = "ERR270";

                            spIn.CreditStatus = 3;  // 3:Exception 
                            spIn.Note = ex.Message;
                            var sp_re2 = MonSubsSp.sp_SetSubsCreditStatus(spIn, ref spErrCode);

                            logger.Error($"IDNO:{item.IDNO} -- MonthlyRentId:{item.MonthlyRentId} -- Credit Error:{ex.Message}");
                        }
                    }
                    #endregion
                    #region 建立月租
                    if (flag)
                    {
                        string spName = "usp_AutoSubscribeJob_I01";
                        SPInput_AutoSubscribeJob_I01 spInput = new SPInput_AutoSubscribeJob_I01()
                        {
                            IDNO = item.IDNO,
                            MonthlyRentId = item.MonthlyRentId,
                            MonProjID = item.MonProjID,
                            MonProPeriod = item.MonProPeriod,
                            ShortDays = item.ShortDays,
                            SubsNxtID = item.SubsNxtID,
                            IsMotor = item.IsMotor,
                            NxtMonSetID = item.NxtMonSetID,
                            PayTypeId = 5,
                            InvoTypeId = item.InvoiceID,
                            MerchantTradeNo = MerchantTradeNo,
                            TaishinTradeNo = TransactionNo,
                            LogID = LogID,
                        };
                        SPOut_AutoSubscribeJob_I01 spOut = new SPOut_AutoSubscribeJob_I01();
                        SQLHelper<SPInput_AutoSubscribeJob_I01, SPOut_AutoSubscribeJob_I01> sqlHelp = new SQLHelper<SPInput_AutoSubscribeJob_I01, SPOut_AutoSubscribeJob_I01>(connetStr);
                        flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                        baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                        if (flag)
                        {
                            NewMonthlyRentID = spOut.NewMonthlyRentID;
                        }
                    }
                    #endregion
                    #region 履保
                    if (flag)
                    {
                        #region 取得會員資料
                        string spName = "usp_GetMemberData";
                        SPInput_GetMemberData spMemberDataInput = new SPInput_GetMemberData()
                        {
                            IDNO = item.IDNO,
                            Token = "",
                            LogID = LogID,
                            CheckToken = 0
                        };
                        SPOutput_Base SPOutputBase = new SPOutput_Base();
                        SQLHelper<SPInput_GetMemberData, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMemberData, SPOutput_Base>(connetStr);
                        List<RegisterData> ListOut = new List<RegisterData>();
                        DataSet ds = new DataSet();
                        flag = sqlHelp.ExeuteSP(spName, spMemberDataInput, ref SPOutputBase, ref ListOut, ref ds, ref lstError);
                        baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                        #endregion
                        if (flag)
                        {
                            if (ListOut != null && ListOut.Count > 0)
                            {
                                MemberData = ListOut.FirstOrDefault();
                                try
                                {
                                    string xerrCode = "";
                                    string xerrMsg = "";
                                    ICF_TSIB_Escrow_Type spInput = new ICF_TSIB_Escrow_Type()
                                    {
                                        IDNO = item.IDNO,
                                        Name = MemberData.MEMCNAME,
                                        PhoneNo = MemberData.MEMTEL,
                                        Email = MemberData.MEMEMAIL,
                                        Amount = PeriodPayPrice,
                                        UseType = 0,
                                        MonthlyNo = NewMonthlyRentID,
                                        PRGID = funName
                                    };
                                    var xFlag = MonSubsCommon.TSIB_Escrow_Month(spInput, ref xerrCode, ref xerrMsg);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error($"IDNO:{item.IDNO} -- NewMonthlyRentId:{NewMonthlyRentID} -- Escrow Error:{ex.Message}");
                                }
                            }
                        }
                    }
                    #endregion
                    #region 發票
                    if (flag && PeriodPayPrice > 0)
                    {
                        try
                        {
                            string INVNO = "";
                            WebAPIInput_MonthlyRentSave wsInput = new WebAPIInput_MonthlyRentSave()
                            {
                                CUSTID = item.IDNO,
                                CUSTNM = MemberData.MEMCNAME,
                                EMAIL = MemberData.MEMEMAIL,
                                MonRentID = NewMonthlyRentID,
                                MonProjID = item.MonProjID,
                                MonProPeriod = item.MonProPeriod,
                                ShortDays = item.ShortDays,
                                NowPeriod = 1, //第一期固定寫1
                                SDATE = item.EndDate.ToString("yyyyMMdd"),
                                EDATE = item.EndDate.AddDays(30).ToString("yyyyMMdd"),
                                IsMoto = item.IsMotor,
                                RCVAMT = PeriodPayPrice,
                                UNIMNO = item.UNIMNO,
                                CARDNO = CreditCardNo,
                                AUTHCODE = AuthCode,
                                NORDNO = TransactionNo,
                                INVKIND = item.InvoiceType.ToString(),
                                CARRIERID = item.CARRIERID,
                                NPOBAN = item.NPOBAN,
                                INVTITLE = "",
                                INVADDR = ""
                            };
                            wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>();
                            wsInput.tbPaymentDetail.Add(new NPR130SavePaymentList()
                            {
                                PAYMENTTYPE = "1",
                                PAYTYPE = "4",
                                PAYAMT = PeriodPayPrice,
                                PORDNO = TransactionNo,
                                PAYMEMO = "月租訂閱制"
                            });
                            WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();
                            var xflag = EasyRentApi.MonthlyRentSave(wsInput, ref wsOutput);
                            if (wsOutput.Result == false)
                            {
                                xflag = false;
                                logger.Error($"IDNO:{item.IDNO} -- 發票開立失敗! MonthlyRentID:{wsInput.MonRentID.ToString()}");
                            }
                            else
                            {
                                INVNO = wsOutput.Data[0].INVNO;
                            }
                            if (!string.IsNullOrEmpty(INVNO))
                            {
                                //寫入發票資訊
                                string sp_errCode = "";
                                var spin = new SPInput_SaveInvno()
                                {
                                    IDNO = item.IDNO,
                                    LogID = LogID,
                                    MonProjID = item.MonProjID,
                                    MonProPeriod = item.MonProPeriod,
                                    ShortDays = item.ShortDays,
                                    NowPeriod = 1,  // 寫死第一期
                                    PayTypeId = 0,  // 不知道幹嘛用的
                                    InvoTypeId = item.InvoiceID,
                                    InvoiceType = item.InvoiceType.ToString(),
                                    CARRIERID = item.CARRIERID,
                                    UNIMNO = item.UNIMNO,
                                    NPOBAN = item.NPOBAN,
                                    Invno = INVNO,
                                    InvoicePrice = PeriodPayPrice,
                                    InvoiceDate = DateTime.Now.ToString("yyyyMMdd"),
                                    PRGID = funName
                                };

                                xflag = MonSubsSp.sp_SaveSubsInvno(spin, ref sp_errCode);
                                if (!xflag)
                                {
                                    logger.Error($"IDNO:{item.IDNO} -- spError={sp_errCode}");
                                }
                            }
                            else
                            {
                                //資料寫入錯誤紀錄log TB_MonthlyInvErrLog
                                string sp_errCode = "";
                                var spInput = new SPInput_InsMonthlyInvErr()
                                {
                                    ApiInput = JsonConvert.SerializeObject(wsInput),
                                    IDNO = item.IDNO,
                                    LogID = LogID,
                                    MonthlyRentID = NewMonthlyRentID,
                                    MonProjID = item.MonProjID,
                                    MonProPeriod = item.MonProPeriod,
                                    ShortDays = item.ShortDays,
                                    NowPeriod = 1,
                                    PayTypeId = 0,  // 不知道幹嘛用的
                                    InvoTypeId = item.InvoiceID,
                                    InvoiceType = item.InvoiceType.ToString(),
                                    CARRIERID = item.CARRIERID,
                                    UNIMNO = item.UNIMNO,
                                    NPOBAN = item.NPOBAN,
                                    INVAMT = PeriodPayPrice,
                                    PRGID = funName,
                                    RtnCode = wsOutput?.RtnCode ?? "-4",
                                    RtnMsg = wsOutput?.Message ?? ""
                                };

                                xflag = MonSubsSp.sp_InsMonthlyInvErr(spInput, ref sp_errCode);
                                if (!xflag)
                                {
                                    logger.Error($"IDNO:{item.IDNO} -- spError={sp_errCode}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"IDNO:{item.IDNO} -- NewMonthlyRentId:{NewMonthlyRentID} -- Invoce Error:{ex.Message}");
                        }
                    }
                    #endregion
                }
            }
            #endregion
            #endregion

            #region 寫入錯誤Log
            if (!flag && !isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
