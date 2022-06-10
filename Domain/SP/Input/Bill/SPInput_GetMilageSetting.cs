using System;

namespace Domain.SP.Input.Bill
{
    /// <summary>
    /// 取里程
    /// </summary>
    public class SPInput_GetMilageSetting : SPInput_Base
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
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 起日
        /// </summary>
        public DateTime SDate { set; get; }

        /// <summary>
        /// 迄日
        /// </summary>
        public DateTime EDate { set; get; }
    }
}