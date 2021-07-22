using System;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_GetMonthlyMain : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? SD { get; set; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? ED { get; set; }
        /// <summary>
        /// 點數狀態
        /// </summary>
        public int hasPointer { get; set; }

    }
}
