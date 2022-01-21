using Domain.Common;
using Domain.SP.Input.MonthlyRent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.MonthlyRent;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class MonthlyPayInvJobController : ApiController
    {
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        public async Task<Dictionary<string, object>> DoMonthlyPayInvJob([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MonthlyPayInvJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string Contentjson = "";
            Token token = null;
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            string Access_Token = "";
            bool isGuest = true;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var msp = new MonSubsSp();
            var trace = new TraceCom();
            var apiInput = new IAPI_MonthlyPayInv();
            var apiOutput = new OAPI_MonthlyPayInv();
            var carRepo = new CarRentRepo();
            SPOut_GetMonthlyPayInv sp_re = null;
            string InvNo = "";
            HiEasyRentAPI EasyRentApi = new HiEasyRentAPI();
            var mem = new Domain.MemberData.RegisterData();
            var mscom = new MonSubsCommon();
            #endregion

            trace.traceAdd("apiIn", value);

            #region 防呆
            bool flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MonthlyPayInv>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                //必填檢查
                if (apiInput.MonthlyRentId == 0 || string.IsNullOrWhiteSpace(apiInput.IdNo))
                {
                    flag = false;
                    errCode = "ERR900"; //必要參數遺漏
                }
                trace.FlowList.Add("防呆");
            }
            #endregion
            #region TB

            #region 產製發票資料
            if (flag)
            {

                SPOutput_Base spOut = new SPOutput_Base();
                string spName = "usp_GetMonthlyPayInvData";
                var spInput = new SPInput_GetMonthlyPayInv
                {
                    IdNo = apiInput.IdNo,
                    MonthlyRentId = apiInput.MonthlyRentId
                };

                SQLHelper<SPInput_GetMonthlyPayInv, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMonthlyPayInv, SPOutput_Base>(connetStr);
                List<SPOut_GetMonthlyPayInv> lstOut = new List<SPOut_GetMonthlyPayInv>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref lstOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                sp_re = (lstOut == null) ? null : (lstOut.Count == 0) ? null : lstOut[0];
            }
            #endregion

            #region 履保
            if (flag)
            {
                var spin = new ICF_TSIB_Escrow_Type()
                {
                    IDNO = apiInput.IdNo,
                    Name = mem.MEMCNAME,
                    PhoneNo = mem.MEMTEL,
                    Email = mem.MEMEMAIL,
                    Amount = sp_re.PreiodPrice,
                    UseType = 0,
                    MonthlyNo = apiInput.MonthlyRentId,
                    PRGID = funName
                };

                try
                {
                    var xFlag = mscom.TSIB_Escrow_Month(spin, ref errCode, ref errMsg);
                    trace.traceAdd("TSIB_Escrow_Month", new { spin, xFlag, errCode, errMsg });
                    trace.FlowList.Add("履保處理");
                }

                catch (Exception ex)
                {
                    trace.traceAdd("Escrow_Exception", new { ex });
                }
            }

            #endregion

            #region 開立發票
            if (flag)
            {
                try
                {
                    WebAPIInput_MonthlyRentSave wsInput = new WebAPIInput_MonthlyRentSave()
                    {
                        CUSTID = apiInput.IdNo,
                        CUSTNM = sp_re.MemCName,
                        EMAIL = sp_re.MemEmail,
                        MonRentID = apiInput.MonthlyRentId,
                        MonProjID = sp_re.MonProjID,
                        MonProPeriod = sp_re.MonProPeriod,
                        ShortDays = sp_re.ShortDays,
                        NowPeriod = sp_re.NowPeriod,
                        SDATE = sp_re.SDate,
                        EDATE = sp_re.EDate,
                        IsMoto = sp_re.IsMoto,
                        RCVAMT = sp_re.PreiodPrice,
                        UNIMNO = sp_re.Unimno,
                        CARDNO = sp_re.CardNumber,
                        AUTHCODE = sp_re.AuthCode,
                        NORDNO = sp_re.TaishinTradeNo,
                        INVKIND = sp_re.InvoiceType.ToString(),
                        CARRIERID = sp_re.CarrierId,
                        NPOBAN = sp_re.Npoban,
                        INVTITLE = "",
                        INVADDR = ""
                    };

                    wsInput.tbPaymentDetail = new List<NPR130SavePaymentList>
                        {
                            new NPR130SavePaymentList()
                            {
                                PAYMENTTYPE = "1",
                                PAYTYPE = "4",
                                PAYAMT = sp_re.PreiodPrice,
                                PORDNO = sp_re.TaishinTradeNo,
                                PAYMEMO = "月租訂閱制"
                            }
                        };
                    WebAPIOutput_MonthlyRentSave wsOutput = new WebAPIOutput_MonthlyRentSave();

                    //發票儲存
                    var xflag = EasyRentApi.MonthlyRentSave(wsInput, ref wsOutput);
                    if (wsOutput.Result == false)
                    {
                        xflag = false;
                        logger.Trace("發票開立失敗!MonthlyRentId=" + wsInput.MonRentID.ToString());
                    }
                    else
                    {
                        InvNo = wsOutput.Data[0].INVNO;
                        logger.Trace("InvNo=" + InvNo);
                    }
                    trace.FlowList.Add("發票開立");

                    if (InvNo != "")
                    {
                        string sp_errCode = "";
                        var spin = new SPInput_SaveInvno()
                        {
                            IDNO = apiInput.IdNo,
                            LogID = LogID,
                            MonProjID = sp_re.MonProjID,
                            MonProPeriod = sp_re.MonProPeriod,
                            ShortDays = sp_re.ShortDays,
                            NowPeriod = sp_re.NowPeriod,
                            PayTypeId = sp_re.PayTypeId,
                            InvoTypeId = sp_re.InvoTypeId,
                            InvoiceType = sp_re.InvoiceType,
                            CARRIERID = sp_re.CarrierId,
                            UNIMNO = sp_re.Unimno,
                            NPOBAN = sp_re.Npoban,
                            Invno = InvNo,
                            InvoicePrice = sp_re.PreiodPrice,
                            InvoiceDate = DateTime.Now.ToString("yyyyMMdd")
                        };

                        xflag = msp.sp_SaveSubsInvno(spin, ref sp_errCode);
                        if (!xflag)
                        {
                            logger.Trace("spError=" + sp_errCode);
                        }
                        trace.traceAdd("sp_SaveSubsInvno", new { InvNo,spin, sp_errCode });
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
                            IDNO = apiInput.IdNo,
                            LogID = LogID,
                            MonthlyRentID = apiInput.MonthlyRentId,
                            MonProjID = sp_re.MonProjID,
                            MonProPeriod = sp_re.MonProPeriod,
                            ShortDays = sp_re.ShortDays,
                            NowPeriod = sp_re.NowPeriod,
                            PayTypeId = (Int64)sp_re.PayTypeId,
                            InvoTypeId = sp_re.InvoTypeId,
                            InvoiceType = sp_re.InvoiceType,
                            CARRIERID = sp_re.CarrierId,
                            UNIMNO = sp_re.Unimno,
                            NPOBAN = sp_re.Npoban,
                            INVAMT = sp_re.PreiodPrice
                        };

                        xflag = msp.sp_InsMonthlyInvErr(spInput, ref sp_errCode);
                        if (!xflag)
                        {
                            logger.Trace("spError=" + sp_errCode);
                        }
                        trace.traceAdd("sp_InsMonthlyInvErr", new { spInput, sp_errCode });
                        trace.FlowList.Add("發票錯誤處理");
                    }
                    apiOutput.PayInvResult = flag ? 1 : 0;
                }
                catch (Exception ex)
                {
                    trace.traceAdd("MonthlyRentSave_Exception", new { ex });
                    //紀錄開立失敗
                    logger.Trace(ex.Message);
                }
            }


            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            carRepo.AddTraceLog(268, funName, trace, flag);
            #endregion
            #region 輸出

            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

    }
}