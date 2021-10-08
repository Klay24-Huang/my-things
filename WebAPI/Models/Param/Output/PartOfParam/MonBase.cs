using System;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 月租選擇
    /// </summary>
    public class MonBase
    {
        /// <summary>
        /// 月租訂單編號
        /// </summary>
        public Int64 MonthlyRentId { get; set; }
        /// <summary>
        /// 月租方案名稱
        /// </summary>
        public string ProjNM { get; set; }
    }
}