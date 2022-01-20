using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 月租基本
    /// </summary>
    public class MonthRentBase
    {
        /// <summary>
        /// 平日費率
        /// </summary>
        public double WorkdayRate { set; get; }
        /// <summary>
        /// 假日費率
        /// </summary>
        public double HoildayRate { set; get; }
    }
}