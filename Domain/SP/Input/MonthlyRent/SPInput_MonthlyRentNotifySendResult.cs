using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_MonthlyRentNotifySendResult
    {
        /// <summary>
        /// 通知流水號
        /// </summary>
        public int SeqNo { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 執行程式
        /// </summary>
        public string PrgName { get; set; }
        /// <summary>
        /// 執行使用者
        /// </summary>
        public string PrgUser { get; set; }
    }
}
