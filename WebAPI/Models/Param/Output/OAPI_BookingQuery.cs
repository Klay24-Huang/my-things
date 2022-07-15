using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_BookingQuery
    {
        /// <summary>
        /// 20220707 ADD BY ADAM REASON.增加是否有有效訂單判斷
        /// 目前是否有效訂單 Y/N
        /// 20220714 ADD BY ADAM REASON.Levy哥說要拿掉
        /// </summary>
        //public string NowOrderFlg { set; get; } = "N";
        public List<ActiveOrderData> OrderObj { set; get; }
    }
}