using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoredByCredit : IAPI_WalletStoreBase
    {
        /// <summary>
        /// 儲值方式(0信用卡,4和泰PAY) 對應TB_MemberData:PayMode
        /// </summary>
        public int StoreType { set; get; }
    }
}