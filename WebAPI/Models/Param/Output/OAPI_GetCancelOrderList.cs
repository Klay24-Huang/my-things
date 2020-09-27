using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetCancelOrderList
    {
        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { set; get; }
        /// <summary>
        /// 取消的訂單資訊
        /// </summary>
        public List<OrderCancelObj> CancelObj { set; get; }
    }
}