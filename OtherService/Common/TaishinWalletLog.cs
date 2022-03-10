using Domain.SP.Input.OtherService.Taishin;
using Domain.SP.Output;
using OtherService.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using WebCommon;

namespace OtherService.Common
{
    public class TaishinWalletLog
    {
 
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        public void InsStoreValueCreateAccountLog(SPInput_InsStoreValueCreateAccountLog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsStoreValueCreateAccountLog, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsStoreValueCreateAccountLog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsStoreValueCreateAccountLog);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
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

        }

        public void InsPayTransactionLog(SPInput_InsPayTransactionLog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsPayTransactionLog, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsPayTransactionLog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsPayTransactionLog);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            
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

        }

        public void InsTransferStoreValueCreateAccountLog(SPInput_InsTransferStoreValueLog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsTransferStoreValueLog, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsTransferStoreValueLog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = new ObjType().GetSPName(ObjType.SPType.InsTransferStoreValueLog);
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            if (flag)
            {
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }

        }

        public void InsWalletStoreShopLog(SPInput_InsWalletStoreShopLog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_InsWalletStoreShopLog, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_InsWalletStoreShopLog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_WalletStoreShop_I02";
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);

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
        }

        public void UpdWalletStoreShopLog(SPInput_UpdWalletStoreShopLog input, ref bool flag, ref string errCode, ref List<ErrorInfo> lstError)
        {
            SQLHelper<SPInput_UpdWalletStoreShopLog, SPOutput_Base> SqlHelper = new SQLHelper<SPInput_UpdWalletStoreShopLog, SPOutput_Base>(connetStr);
            SPOutput_Base spOut = new SPOutput_Base();
            string SPName = "usp_WalletStoreShop_U01";
            flag = SqlHelper.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
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
        }
    }
}
