using Domain.SP.Input.Hotai;
using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.OtherService.Taishin;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommon;

namespace OtherService.Common
{
    /// <summary>
    /// 共用
    /// </summary>
    public class WebAPILogCommon
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 寫入呼叫第三方api log
        /// </summary>
        /// <param name="input"></param>
        /// <param name="flag"></param>
        /// <param name="errCode"></param>
        /// <param name="lstError"></param>
        public void InsWebAPILog(SPInut_WebAPILog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInut_WebAPILog, SPOutput_Base> SqlHelper = new SQLHelper<SPInut_WebAPILog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsWebAPILog);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }

        }
        /// <summary>
        /// 寫入刷卡
        /// </summary>
        public void InsCreditAuthData(SPInput_InsTrade input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsTrade, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsTrade, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }

        /// <summary>
        /// 寫入刷卡 有關帳檔版本
        /// </summary>
        public void InsCreditAuthDataforClose(SPInput_InsTradeForClose input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsTradeForClose, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsTradeForClose, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_InsTradeForClose_I01_V20211214";//new ObjType().GetSPName(ObjType.SPType.InsTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }
        /// <summary>
        /// 更新刷卡結果
        /// </summary>
        /// <param name="flag"></param>
        public void UpdCreditAuthData(SPInput_UpdTrade input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_UpdTrade, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_UpdTrade, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.UpdTrade);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }
        /// <summary>
        /// 更新刷卡結果 有關帳檔版本
        /// </summary>
        /// <param name="flag"></param>
        public void UpdCreditAuthDataForClose(SPInput_UpdTradeForClose input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_UpdTradeForClose, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_UpdTradeForClose, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_UPDTradeForClose_U01";
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }

        public void InsCreditRefundData(SPInput_InsTrade input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsTrade, SPOutput_RefundBase> SqlHelper = new SQLHelper<SPInput_InsTrade, SPOutput_RefundBase>(connetStr);
            SPOutput_RefundBase spOut = new SPOutput_RefundBase();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsTradeRefund);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }
        public Int64 InsCreditRefundDataNew(SPInput_InsTrade input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsTrade, SPOutput_InsTrade> SqlHelper = new SQLHelper<SPInput_InsTrade, SPOutput_InsTrade>(connetStr);
            SPOutput_InsTrade spOut = new SPOutput_InsTrade();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsTradeRefund);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                    return 0;
                }
                else
                {
                    return spOut.TradeRefundID;
                }
            }
            else
            {
                return 0;
            }
        }
        public void UpdCreditRefundData(SPInput_UpdTrade input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_UpdTrade, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_UpdTrade, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.UpdTradeRefund);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }
        }


    }
}
