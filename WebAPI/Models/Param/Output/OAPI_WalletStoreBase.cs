using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_WalletStoreBase
    {
        /// <summary>
        /// 儲值結果(1:成功 0:失敗)
        /// </summary>
        public int StroeResult { get; set; }

        /// <summary>
        /// 儲值金額
        /// </summary>
        public int StoreMoney { get; set; }
      
    }
}