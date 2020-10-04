using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class CarRentBase
    {
        /// <summary>
        /// 多少小時算一天
        /// </summary>
        public int HourOfOneDay { set; get; }
        /// <summary>
        /// 假日一天
        /// </summary>
        public int HoildayPrice { set; get; }
        /// <summary>
        /// 平日一天
        /// </summary>
        public int WorkdayPrice { set; get; }
        /// <summary>
        /// 假日每小時
        /// </summary>
        public int HoildayOfHourPrice { set; get; }
        /// <summary>
        /// 平日每小時
        /// </summary>
        public int WorkdayOfHourPrice { set; get; }
        /// <summary>
        /// 每公里n元
        /// </summary>
        public float MilUnit { set; get; }

    }
}