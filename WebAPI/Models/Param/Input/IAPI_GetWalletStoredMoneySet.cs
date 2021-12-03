using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 錢包儲值-設定資訊
    /// </summary>
    public class IAPI_GetWalletStoredMoneySet
    {
        /// <summary>
        /// 儲值方式(0:信用卡 2:虛擬帳號 3:超商繳費 4:和泰Pay)
        /// </summary>
        public int StoreType { get; set; }
    }
}