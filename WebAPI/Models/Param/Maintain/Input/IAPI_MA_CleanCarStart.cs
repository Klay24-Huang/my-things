using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Maintain.Input
{
    /// <summary>
    /// 整備人員取車
    /// </summary>
    public class IAPI_MA_CleanCarStart
    {
        /// <summary>
        /// 使用者id
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        public Int64 OrderNum { set; get; }
    }
}