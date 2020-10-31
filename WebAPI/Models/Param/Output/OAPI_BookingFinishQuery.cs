using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 已完成的訂單查詢
    /// </summary>
    public class OAPI_BookingFinishQuery
    {
        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { set; get; }
        /// <summary>
        /// 租用年 
        /// 20201029 ADD BY ADAM REASON.移到完成訂單資訊內
        /// </summary>
        //public string RentYear { set; get; }

        /// <summary>
        /// 完成的訂單資訊
        /// </summary>
        public List<OrderFinishObj> OrderFinishObjs { set; get; }
    }
}