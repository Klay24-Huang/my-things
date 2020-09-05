using Domain.SP.Input.OtherService.Common;
using Domain.SP.Output;
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
    }
}
