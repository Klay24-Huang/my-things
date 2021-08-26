﻿using Domain.SP.Input.Bill;
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
using Domain.TB;
using Reposotory.Implement;
using WebAPI.Models.Param.CusFun.Input;
using Domain.WebAPI.Input.Taishin.Wallet;
using Newtonsoft.Json;
using Domain.WebAPI.output.Taishin.Wallet;
using Domain.MemberData;
using Domain.SP.Input.Member;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using NLog;
using Domain.SP.Output.Wallet;
using Domain.SP.Input.Wallet;

namespace WebAPI.Service
{
    public class WalletService
    {
    }

    public class WalletSp
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public List<SPOut_GetWalletStoreTradeTransHistory> sp_GetWalletStoreTradeTransHistory(SPInput_GetWalletStoreTradeTransHistory spInput, ref string errCode)
        {
            var re = new List<SPOut_GetWalletStoreTradeTransHistory>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletStoreTradeTransHistory);
                string SPName = "usp_GetWalletStoreTradeTransHistory_Q1";
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.SD,
                        spInput.ED
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
                        re = objUti.ConvertToList<SPOut_GetWalletStoreTradeTransHistory>(ds1.Tables[0]);
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

        public List<SPOut_WalletTransferCheck> sp_WalletTransferCheck(SPInput_WalletTransferCheck spInput, ref string errCode)
        {
            var re = new List<SPOut_WalletTransferCheck>();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.WalletTransferCheck);
                string SPName = "usp_WalletTransferCheck_Q1";
                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID,
                        spInput.PhoneNo,
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
                        re = objUti.ConvertToList<SPOut_WalletTransferCheck>(ds1.Tables[0]);
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[1]);
                        errCode = re_db.ErrorCode;
                    }
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

        public bool sp_WalletStoreTradeHistoryHidden(SPInput_WalletStoreTradeHistoryHidden spInput, ref string errCode)
        {
            bool flag = false;
            //string spName = new ObjType().GetSPName(ObjType.SPType.SetSubsNxt);
            string spName = "usp_WalletStoreTradeHistoryHidden_U1";

            var lstError = new List<ErrorInfo>();
            var spOut = new SPOut_WalletStoreTradeHistoryHidden();
            SQLHelper<SPInput_WalletStoreTradeHistoryHidden, SPOut_WalletStoreTradeHistoryHidden> sqlHelp = new SQLHelper<SPInput_WalletStoreTradeHistoryHidden, SPOut_WalletStoreTradeHistoryHidden>(connetStr);
            bool spFlag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (spFlag && spOut != null)
            {
                if (spOut.ErrorCode != "0000")
                    errCode = spOut.ErrorCode;
                flag = spOut.xError == 0;
            }

            return flag;
        }

        public bool sp_SetWalletTrade(SPInput_SetWalletTrade spInput, ref string errCode)
        {
            bool flag = false;
            //string spName = new ObjType().GetSPName(ObjType.SPType.SetWalletTrade);
            string spName = "usp_SetWalletTrade_U1";

            var lstError = new List<ErrorInfo>();
            var spOut = new SPOut_SetWalletTrade();
            SQLHelper<SPInput_SetWalletTrade, SPOut_SetWalletTrade> sqlHelp = new SQLHelper<SPInput_SetWalletTrade, SPOut_SetWalletTrade>(connetStr);
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

    public class WalletMap
    {
        public List<OAPI_WalletStoreTradeTrans> FromSPOut_GetWalletStoreTradeTransHistory(List<SPOut_GetWalletStoreTradeTransHistory> sour)
        {
            var re = new List<OAPI_WalletStoreTradeTrans>();
            if (sour != null && sour.Count() > 0)
            {
                re = (from a in sour
                      select new OAPI_WalletStoreTradeTrans
                      {
                          ORGID = a.ORGID,
                          IDNO = a.IDNO,
                          SEQNO = a.SEQNO,
                          F_INFNO = a.F_INFNO,
                          TradeYear = Convert.ToDateTime(a.TradeDate).Year,
                          TradeDate = Convert.ToDateTime(a.TradeDate).ToString("MM/dd"),
                          TradeTime = Convert.ToDateTime(a.TradeDate).ToString("HH:mm"),  
                          TradeTypeNm = a.CodeName,
                          TradeNote = a.TradeNote,
                          TradeAMT = Convert.ToInt32(a.TradeAMT),
                          ShowFLG = 1
                      }).ToList(); 
            }

            return re;
        }
    
    }
}