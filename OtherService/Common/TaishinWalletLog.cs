﻿using Domain.SP.Input.OtherService.Taishin;
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
    class TaishinWalletLog
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
                if (spOut.Error == 1)
                {
                    flag = false;
                }
            }

        }
    }
}
