using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_BE_GiveGiftMins
    {
        /// <summary>
        /// 會員流水號
        /// </summary>
        public string MEMRFNBR { get; set; }
        /// <summary>
        /// 贈與分鐘數
        /// </summary>
        public int GIFTMINS { get; set; }
        /// <summary>
        /// 優惠名稱
        /// </summary>
        public string COUPONNAME { get; set; }
        /// <summary>
        /// 贈點類型(01:汽車, 02:機車)
        /// </summary>
        public string GIFTTYPE { get; set; }
        /// <summary>
        /// 起日
        /// </summary>
        public string SDATE { get; set; }
        /// <summary>
        /// 迄日
        /// </summary>
        public string EDATE { get; set; }
    }
}