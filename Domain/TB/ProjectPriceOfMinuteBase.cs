using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 以分計費
    /// </summary>
   public class ProjectPriceOfMinuteBase
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 基本分鐘數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 平日基本費
        /// </summary>
        public int BaseMinutesPrice { set; get; }
        /// <summary>
        /// 假日基本費
        /// </summary>
        public int BaseMinutesPriceH { set; get; }
        /// <summary>
        /// 平日上限金額
        /// </summary>
        public int MaxPrice { set; get; }
        /// <summary>
        /// 假日上限金額
        /// </summary>
        public int MaxPriceH { set; get; }
        /// <summary>
        /// 專案平日價(每分鐘)
        /// </summary>
        public float Price { set; get; }
        /// <summary>
        /// 專案假日價(每分鐘)
        /// </summary>
        public float PriceH { set; get; }
    }
}
