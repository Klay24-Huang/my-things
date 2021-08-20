using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletInfoCheck
    {
        /// <summary>
        /// 驗證結果(1成功,0失敗)
        /// </summary>
        public int CkResult { get; set; } = 0;
        /// <summary>
        /// 錢包剩餘金額
        /// </summary>
        public int WalletAmount { get; set; } = 0;
        /// <summary>
        /// 當月入賬總金額
        /// </summary>
        public int MonTransIn { get; set; } = 0;
    }
}