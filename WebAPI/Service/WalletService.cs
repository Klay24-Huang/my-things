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
using Domain.SP.Input.OtherService.Taishin;
using WebAPI.Models.Param.Input;

namespace WebAPI.Service
{
    public class WalletService
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public bool CheckStoreAmtLimit(int StoreMoney, string IDNO, long LogID, string Access_Token, ref bool flag, ref string errCode) 
        {
            
            CommonFunc baseVerify = new CommonFunc();
            string spName = "usp_GetWallet_Q01";
            SPInput_GetWallet spInput = new SPInput_GetWallet()
            {
                IDNO = IDNO,
                LogID = LogID,
                Token = Access_Token
            };
            SPOutput_GetWallet sPOutput_GetWallet = new SPOutput_GetWallet();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SQLHelper<SPInput_GetWallet, SPOutput_GetWallet> sqlHelp = new SQLHelper<SPInput_GetWallet, SPOutput_GetWallet>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref sPOutput_GetWallet, ref lstError);
            baseVerify.checkSQLResult(ref flag, sPOutput_GetWallet.Error, sPOutput_GetWallet.ErrorCode, ref lstError, ref errCode);

            if (flag && sPOutput_GetWallet != null)
            {
                //錢包現存餘額上限為5萬元
                if (StoreMoney + sPOutput_GetWallet.WalletBalance > 50000)
                {
                    flag = false;
                    errCode = "ERR282";
                }
                //錢包單月儲值(包括受贈)上限為30萬元
                else if (StoreMoney + sPOutput_GetWallet.MonthlyTransAmount > 300000)
                {
                    flag = false;
                    errCode = "ERR280";
                }
            }

            return flag;
        }
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

        public SPOut_GetPayInfoReturnCar sp_GetPayInfoReturnCar(SPInput_GetPayInfoReturnCar spInput, ref string errCode)
        {
            var re = new SPOut_GetPayInfoReturnCar();

            try
            {
                //string SPName = new ObjType().GetSPName(ObjType.SPType.GetPayInfoReturnCar);
                string SPName = "usp_GetPayInfoReturnCar_Q1_20210830";
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
                    if (ds1.Tables.Count >= 3)
                    {
                        var CheckoutModes = objUti.ConvertToList<SPOut_GetPayInfoReturnCar_CheckoutModes>(ds1.Tables[0]);
                        var PayInfos = objUti.ConvertToList<SPOut_GetPayInfoReturnCar_PayInfo>(ds1.Tables[1]);
                        if (CheckoutModes != null && CheckoutModes.Count() > 0)
                            re.CheckoutModes = CheckoutModes;
                        if (PayInfos != null && PayInfos.Count() > 0)
                            re.PayInfo = PayInfos.FirstOrDefault();
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

        public bool sp_WalletStore(SPInput_WalletStore spInput, ref string errCode)
        {
            bool flag = false;
            string spName = "usp_WalletStore_U01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_WalletStore, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_WalletStore, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }

        public bool sp_WalletPay(SPInput_WalletPay spInput, ref string errCode)
        {
            bool flag = false;

            string spName = "usp_WalletPay_I01";
            var lstError = new List<ErrorInfo>();
            var spOut = new SPOutput_Base();

            SQLHelper<SPInput_WalletPay, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_WalletPay, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
            return flag;
        }

        /// <summary>
        /// 台新虛擬帳號產生紀錄
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public bool sp_WalletStoreVisualAccount(SPInput_InsWalletStoreVisualAccountLog spInput, ref string errCode)
        {
            bool flag = false;
            string spName = "usp_InsWalletStoreVisualAccountLog_I01";

            var lstError = new List<ErrorInfo>();
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_InsWalletStoreVisualAccountLog, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsWalletStoreVisualAccountLog, SPOutput_Base>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOutput, ref lstError);

            if (flag)
            {
                if (spOutput.Error == 1 || spOutput.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOutput.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }

            return flag;
        }

        public SPOutput_GetCvsPaymentId sp_GetCvsPaymentId(SPInput_GetCvsPaymentId spInput, ref string errCode)
        {
            bool flag = false;

            string spName = "usp_InsTaishinWalletCvsPaymentId_I01";
            var lstError = new List<ErrorInfo>();
            var spOut = new SPOutput_GetCvsPaymentId();

            SQLHelper<SPInput_GetCvsPaymentId, SPOutput_GetCvsPaymentId> sqlHelp = new SQLHelper<SPInput_GetCvsPaymentId, SPOutput_GetCvsPaymentId>(connetStr);
            flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);

            if (flag)
            {
                if (spOut.Error == 1 || spOut.ErrorCode != "0000")
                {
                    flag = false;
                    errCode = spOut.ErrorCode;
                }
            }
            else
            {
                if (lstError.Count > 0)
                {
                    errCode = lstError[0].ErrorCode;
                }
            }
            return spOut;
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
                          SEQNO = a.SEQNO,
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

        public OPAI_GetPayInfoReturnCar FromSPOut_GetPayInfoReturnCar(SPOut_GetPayInfoReturnCar sour)
        {
            var re = new OPAI_GetPayInfoReturnCar();

            if (sour != null)
            {
                if (sour.CheckoutModes != null && sour.CheckoutModes.Count() > 0)
                {
                    var CheckoutModes = (
                           from a in sour.CheckoutModes
                           select new OPAI_GetPayInfoReturnCar_CheckoutMode
                           {
                               CheckoutMode = a.CheckoutMode,
                               CheckoutNM = a.CheckoutNM,
                               CheckoutNote = a.CheckoutNote,
                               IsDef = a.IsDef
                           }).ToList();
                    re.CheckoutModes = CheckoutModes;
                }

                if (sour.PayInfo != null)
                {
                    var a = sour.PayInfo;
                    var PayInfo = new OPAI_GetPayInfoReturnCar_PayInfo();
                    PayInfo.WalletAmount = a.WalletAmount;
                    PayInfo.CreditStoreAmount = a.CreditStoreAmount;
                    re.PayInfo = PayInfo;
                }
            }

            return re;
        }
    }
}