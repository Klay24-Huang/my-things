using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 折抵時數轉贈
    /// </summary>
    public class IAPI_BonusGiving
    {

        /// <summary>
        /// 受贈者身份證
        /// </summary>
        public string TargetID { set; get; }
        /// <summary>
        /// 贈送分鐘數
        /// </summary>
        public int TransMins { set; get; }
        /// <summary>
        /// 01(汽車)/02(機車)
        /// </summary>
        public string GiftType { get; set; }
    }
}