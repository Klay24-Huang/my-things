using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class MotorRentBase
    {
        /// <summary>
        /// 基本時數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 基本費
        /// </summary>
        public int BaseMinutePrice { set; get; }
        /// <summary>
        /// 每分鐘多少費用
        /// </summary>
        public float MinuteOfPrice { set; get; }
    }
}