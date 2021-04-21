using Domain.SP.Input.Bill;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.Rent;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebAPI.Utils;
using WebCommon;
using System.Configuration;
using WebAPI.Models.BaseFunc;
using Domain.SP.Output.Subscription;
using WebAPI.Models.Param.Bill.Input;
using Domain.WebAPI.output.Taishin;
using OtherService;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin;
using System.Threading;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Models.Param.Output;

namespace WebAPI.Models.BillFunc
{
    /// <summary>
    /// 月租訂閱制
    /// </summary>
    public class MonSubsCommon
    {
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();

        public bool Month_TSIBTrade(string IDNO, ref WebAPIOutput_Auth WSAuthOutput, ref int Amount, ref string errCode)
        {
            var TSIB_In = new IFN_TSIBCardTrade();
            TSIB_In.IDNO = IDNO;
            TSIB_In.MerchantTradeNo = string.Format("{0}M_{1}",IDNO, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            TSIB_In.ProdNm = string.Format("{0}月租", IDNO);           
            return TSIBCardTrade(TSIB_In,ref WSAuthOutput,ref Amount,ref errCode);
        }

        /// <summary>
        /// 台新信用卡交易
        /// </summary>
        private bool TSIBCardTrade(IFN_TSIBCardTrade sour, ref WebAPIOutput_Auth WSAuthOutput, ref int Amount, ref string errCode)
        {
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var baseVerify = new CommonFunc();
            string FunNm = System.Reflection.MethodBase.GetCurrentMethod().Name;
            int FunId = SiteUV.GetFunId(FunNm);
            bool flag = true;
            trace.traceAdd("FnIn", new { sour, WSAuthOutput, Amount, errCode });

            try
            {               
                if (string.IsNullOrWhiteSpace(sour.IDNO) ||
                    string.IsNullOrWhiteSpace(sour.ProdNm) ||
                    string.IsNullOrWhiteSpace(sour.MerchantTradeNo) ||
                    Amount <= 0)
                {
                    flag = false;
                    errCode = "ERR257";
                }

                trace.FlowList.Add("欄位驗證");
                trace.traceAdd("FnInCk", new { flag, errCode });

                #region 台新信用卡
                if (flag)
                {
                    //送台新查詢
                    TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                    PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                    {
                        ApiVer = ApiVerOther,
                        ApposId = TaishinAPPOS,
                        RequestParams = new GetCreditCardListRequestParamasData()
                        {
                            MemberId = sour.IDNO,
                        },
                        Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                        TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                        TransNo = string.Format("{0}_{1}", sour.IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))
                    };

                    //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                    string errMsg = "";
                    DataSet ds = Common.getBindingList(sour.IDNO, ref flag, ref errCode, ref errMsg);

                    if (flag)
                    {
                        bool hasFind = false;
                        string CardToken = "";
                        //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            CardToken = ds.Tables[0].Rows[0]["CardToken"].ToString();
                            hasFind = true;
                            trace.FlowList.Add("有綁卡");
                        }

                        #region 直接授權
                        if (hasFind)//有找到，可以做扣款
                        {
                            Thread.Sleep(1000);
                            if (Amount > 0)
                            {
                                Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                                {
                                    Amount = Amount.ToString() + "00",
                                    Name = sour.ProdNm,
                                    NonPoint = "N",
                                    NonRedeem = "N",
                                    Price = Amount.ToString() + "00",
                                    Quantity = "1"
                                };
                                PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                                {
                                    ApiVer = "1.0.2",
                                    ApposId = TaishinAPPOS,
                                    RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                                    {
                                        CardToken = CardToken,
                                        InstallPeriod = "0",
                                        InvoiceMark = "N",
                                        Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                                        MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                                        MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                                        MerchantTradeNo = sour.MerchantTradeNo,
                                        NonRedeemAmt = Amount.ToString() + "00",
                                        NonRedeemdescCode = "",
                                        Remark1 = "",
                                        Remark2 = "",
                                        Remark3 = "",
                                        ResultUrl = BindResultURL,
                                        TradeAmount = Amount.ToString() + "00",
                                        TradeType = "1",
                                        UseRedeem = "N"
                                    },
                                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                                };
                                WSAuthInput.RequestParams.Item.Add(item);
                                trace.FlowList.Add("刷卡交易");

                                flag = WebAPI.DoCreditCardAuthV2(WSAuthInput, sour.IDNO, ref errCode, ref WSAuthOutput);                                
                                trace.traceAdd("DoCreditCardAuthV2", new { sour.IDNO, errCode, WSAuthInput, WSAuthOutput });

                                if (WSAuthOutput.RtnCode != "1000")
                                {
                                    flag = false;
                                    errCode = "ERR197";
                                }
                                //修正錯誤偵測
                                if (WSAuthOutput.RtnCode == "1000" && WSAuthOutput.ResponseParams.ResultCode != "1000")
                                {
                                    flag = false;
                                    errCode = "ERR197";
                                }
                            }
                        }
                        else
                        {
                            flag = false;
                            errCode = "ERR195";
                        }
                        #endregion
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR730";
                    }
                    ds.Dispose();
                }
                #endregion

                trace.traceAdd("errCodeFinal", errCode);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
            }

            if (string.IsNullOrWhiteSpace(trace.BaseMsg))
                carRepo.AddTraceLog(FunId, FunNm, eumTraceType.exception, trace);
            else
            {
                if(flag)
                    carRepo.AddTraceLog(FunId, FunNm, eumTraceType.mark, trace);
                else
                    carRepo.AddTraceLog(FunId, FunNm, eumTraceType.followErr, trace);
            }

            return flag;
        }
    }

    /// <summary>
    /// 月租訂閱制sp
    /// </summary>
    public class MonSubsSp
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 取得月租列表
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public SPOutput_GetMonthList sp_GetMonthList(SPInput_GetMonthList spInput, ref string errCode)
        {
            var re = new SPOutput_GetMonthList();
            re.MyMonths = new List<SPOutput_GetMonthList_Month>();
            re.AllMonths = new List<SPOutput_GetMonthList_Month>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthList);
                string SPName = "usp_GetMonthList_U1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonType
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 3)
                    {
                        re.MyMonths = objUti.ConvertToList<SPOutput_GetMonthList_Month>(ds1.Tables[0]);
                        re.AllMonths = objUti.ConvertToList<SPOutput_GetMonthList_Month>(ds1.Tables[1]);
                    }
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errCode = re_db.ErrorCode;
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPOut_GetMySubs sp_GetMySubs(SPInput_GetMySubs spInput, ref string errCode)
        {
            var re = new SPOut_GetMySubs();
            re.Months = new List<SPOut_GetMySubs_Month>();
            re.Codes = new List<SPOut_GetMySubs_Code>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthList);
                string SPName = "usp_GetMySubs_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonType,
                        spInput.SetNow
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 3)
                    {
                        var months = objUti.ConvertToList<SPOut_GetMySubs_Month>(ds1.Tables[0]);
                        if (months != null && months.Count() > 0)
                            re.Months = months;

                        var codes = objUti.ConvertToList<SPOut_GetMySubs_Code>(ds1.Tables[1]);
                        if (codes != null && codes.Count() > 0)
                            re.Codes = codes;
                    }
                    else 
                    {
                        int lstIndex = ds1.Tables.Count - 1;
                        if (lstIndex > 0)
                        {
                            var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[lstIndex]);
                            if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                                errCode = re_db.ErrorCode;
                        }
                        else
                            errCode = "ERR908";
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPOut_GetChgSubsList sp_GetChgSubsList(SPInput_GetChgSubsList spInput, ref string errCode)
        {
            var re = new SPOut_GetChgSubsList();
            re.OtrCards = new List<SPOut_GetChgSubsList_Card>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetChgSubsList);
                string SPName = "usp_GetChgSubsList_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonProjID,
                        spInput.MonProPeriod,
                        spInput.ShortDays,
                        spInput.SetNow
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 3)
                    {
                        var myCards = objUti.ConvertToList<SPOut_GetChgSubsList_Card>(ds1.Tables[0]);
                        if (myCards != null && myCards.Count() > 0)
                            re.NowCard = myCards.FirstOrDefault();

                        var otrCards = objUti.ConvertToList<SPOut_GetChgSubsList_Card>(ds1.Tables[1]);
                        if (otrCards != null && otrCards.Count() > 0)
                            re.OtrCards = otrCards;
                    }
                    else
                    {
                        int lstIndex = ds1.Tables.Count - 1;
                        if (lstIndex > 0)
                        {
                            var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[lstIndex]);
                            if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                                errCode = re_db.ErrorCode;
                        }
                        else
                            errCode = "ERR908";
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SPOut_GetUpSubsList sp_GetUpSubsList(SPInput_GetUpSubsList spInput, ref string errCode)
        {
            var re = new SPOut_GetUpSubsList();
            re.Cards = new List<SPOut_GetUpSubsList_Card>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetUpSubsList);
                string SPName = "usp_GetUpSubsList_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonProjID,
                        spInput.MonProPeriod,
                        spInput.ShortDays,
                        spInput.SetNow
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                    {
                        var cards = objUti.ConvertToList<SPOut_GetUpSubsList_Card>(ds1.Tables[0]);
                        if (cards != null && cards.Count() > 0)
                            re.Cards = cards;
                    }
                    else
                    {
                        int lstIndex = ds1.Tables.Count - 1;
                        if (lstIndex > 0)
                        {
                            var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[lstIndex]);
                            if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                                errCode = re_db.ErrorCode;
                        }
                        else
                            errCode = "ERR908";
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得合約明細
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public SPOut_GetSubsCNT sp_GetSubsCNT(SPInput_GetSubsCNT spInput, ref string errCode)
        {
            var re = new SPOut_GetSubsCNT();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetSubsCNT);
                string SPName = "usp_GetSubsCNT_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonProjID,
                        spInput.MonProPeriod,
                        spInput.ShortDays,
                        spInput.SetNow
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 3)
                    {
                        var nowCards = objUti.ConvertToList<SPOut_GetSubsCNT_NowCard>(ds1.Tables[0]);
                        if (nowCards != null && nowCards.Count() > 0)
                            re.NowCard = nowCards.FirstOrDefault();

                        var nxtCards = objUti.ConvertToList<SPOut_GetSubsCNT_NxtCard>(ds1.Tables[1]);
                        if (nxtCards != null && nxtCards.Count() > 0)
                            re.NxtCard = nxtCards.FirstOrDefault();
                    }
                    else
                    {
                        int lstIndex = ds1.Tables.Count - 1;
                        if (lstIndex > 0)
                        {
                            var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[lstIndex]);
                            if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                                errCode = re_db.ErrorCode;
                        }
                        else
                            errCode = "ERR908";
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得月租Group
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public List<SPOut_GetMonthGroup> sp_GetMonthGroup(SPInput_GetMonthGroup spInput, ref string errCode)
        {
            var re = new List<SPOut_GetMonthGroup>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetMonthGroup);
                string SPName = "usp_GetMonthGroup_Q01";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.MonProjID,
                        spInput.SetNow
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                        re = objUti.ConvertToList<SPOut_GetMonthGroup>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errCode = re_db.ErrorMsg;
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                errCode = ex.ToString();
                throw ex;
            }
        }

        public List<SPOut_GetBuyNowInfo> sp_GetBuyNowInfo(SPInput_GetBuyNowInfo spInput, ref string errCode)
        {
            var re = new List<SPOut_GetBuyNowInfo>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetBuyNowInfo);
                string SPName = "usp_GetBuyNowInfo_Q1";//hack: fix spNm
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID
                    },
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                        re = objUti.ConvertToList<SPOut_GetBuyNowInfo>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errCode = re_db.ErrorMsg;
                    }
                }

                return re;
            }
            catch (Exception ex)
            {
                errCode = ex.ToString();
                throw ex;
            }
        }

        public bool sp_CreateSubsMonth(SPInput_CreateSubsMonth spInput, ref string errCode)
        {
            bool flag = false;
            //string spName = new ObjType().GetSPName(ObjType.SPType.CreateSubsMonth);
            string spName = "usp_CreateSubsMonth_U1";//hack: fix spNm

            var lstError = new List<ErrorInfo>();
            var spOutBase = new SPOutput_Base();
            var spOut = new SPOut_CreateSubsMonth();
            SQLHelper<SPInput_CreateSubsMonth, SPOut_CreateSubsMonth> sqlHelp = new SQLHelper<SPInput_CreateSubsMonth, SPOut_CreateSubsMonth>(connetStr);
            bool spFlag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (spFlag && spOut != null)
            {
                if(spOut.ErrorCode != "0000")
                  errCode = spOut.ErrorCode;
                flag = spOut.xError == 0;
            }

            return flag;
        }

        public bool sp_SetSubsNxt(SPInput_SetSubsNxt spInput, ref string errCode)
        {
            bool flag = false;
            //string spName = new ObjType().GetSPName(ObjType.SPType.SetSubsNxt);
            string spName = "usp_SetSubsNxt_U1";//hack: fix spNm

            var lstError = new List<ErrorInfo>();
            var spOut = new SPOut_SetSubsNxt();
            SQLHelper<SPInput_SetSubsNxt, SPOut_SetSubsNxt> sqlHelp = new SQLHelper<SPInput_SetSubsNxt, SPOut_SetSubsNxt>(connetStr);
            bool spFlag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (spFlag && spOut != null)
            {
                if (spOut.ErrorCode != "0000")
                    errCode = spOut.ErrorCode;
                flag = spOut.xError == 0;
            }

            return flag;
        }
    }

    /// <summary>
    /// 訂閱制VM相關Mapping
    /// </summary>
    public class MonSunsVMMap
    {
        public List<MonCardParam> FromSPOutput_GetMonthList_Month(List<SPOutput_GetMonthList_Month> sour)
        {
            var re = new List<MonCardParam>();
            if(sour != null && sour.Count()>0)
            {
               re = (from a in sour
                             select new MonCardParam
                             {
                                 MonProjID = a.MonProjID,
                                 MonProjNM = a.MonProjNM,
                                 MonProPeriod = a.MonProPeriod,
                                 ShortDays = a.ShortDays,
                                 PeriodPrice = a.PeriodPrice,
                                 IsMoto = a.IsMoto,
                                 CarWDHours = a.CarWDHours,
                                 CarHDHours = a.CarHDHours,
                                 MotoTotalMins = a.MotoTotalMins,
                                 IsDiscount = a.IsDiscount
                             }).ToList();
            }
            return re;
        }
    
        public OAPI_GetSubsCNT_Card FromSPOut_GetSubsCNT_NowCard(SPOut_GetSubsCNT_NowCard sour)
        {
            if (sour != null)
            {
                return new OAPI_GetSubsCNT_Card()
                {
                    MonProjID = sour.MonProjID,
                    MonProPeriod = sour.MonProPeriod,
                    ShortDays = sour.ShortDays,
                    MonProjNM = sour.MonProjNM,
                    CarWDHours = sour.WorkDayHours,
                    CarHDHours = sour.HolidayHours,
                    MotoTotalMins = sour.MotoTotalHours,
                    SD = sour.StartDate.ToString("MM/dd"),
                    ED = sour.EndDate.ToString("MM/dd")
                };
            }
            else
                return null;
        }
    }
}