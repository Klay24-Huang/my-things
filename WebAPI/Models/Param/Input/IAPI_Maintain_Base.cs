using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 維護人員基本input
    /// </summary>
    public class IAPI_Maintain_Base
    {
        /// <summary>
        /// 驗證金鑰
        /// </summary>
        public string CheckKey { set; get; }
    }
}