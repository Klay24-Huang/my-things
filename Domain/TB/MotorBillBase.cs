using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 機車時數與費用相關
    /// </summary>
    public class MotorBillBase
    {
        /// <summary>
        /// 基本時數
        /// </summary>
        public int BaseMinutes { set; get; }
        /// <summary>
        /// 基本費
        /// </summary>
        public int BasePrice { set; get; }
        /// <summary>
        /// 每分鐘n元
        /// </summary>
        public float PerMinutesPrice { set; get; }
        /// <summary>
        /// 每日上限
        /// </summary>
        public int MaxPrice { set; get; }
    }
}
