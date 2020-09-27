using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 折抵時數帳號轉贈確認_輸出參數
    /// </summary>
    public class OAPI_ConfirmBeforeGiving
    {
        /// <summary>
        /// 受贈者身份證號
        /// </summary>
        public string MEMIDNO { set; get; }
        /// <summary>
        /// 受贈者姓名
        /// </summary>
        public string MEMCNAME { set; get; }
    }
}