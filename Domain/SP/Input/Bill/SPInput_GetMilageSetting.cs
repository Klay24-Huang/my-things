using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Bill
{
    /// <summary>
    /// 取里程
    /// </summary>
    public class SPInput_GetMilageSetting
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SDate { set; get; }
        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime EDate { set; get; }
        public Int64 LogID { set; get; }
    }
}
