using System;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_GetCanUseMonthly : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 訂閱制ID
        /// </summary>
        public string MonthlyRentIDs { get; set; }
    }
}