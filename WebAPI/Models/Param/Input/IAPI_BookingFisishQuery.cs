using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 已完成的訂單查詢
    /// </summary>
    public class IAPI_BookingFisishQuery
    {
        /// <summary>
        /// 目前頁數
        /// </summary>
        public int? NowPage { set; get; }
        /// <summary>
        /// 顯示一整年
        /// <para>0:否</para>
        /// <para>20XX代表取出該年度的所有訂單</para>
        /// </summary>
        public int ShowOneYear { set; get; }
    }
}