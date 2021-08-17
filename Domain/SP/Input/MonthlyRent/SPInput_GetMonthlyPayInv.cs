using System;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_GetMonthlyPayInv
    {
        /// <summary>
        /// 月租主檔流水號
        /// </summary>
        public int MonthlyRentId { get; set; }

        /// <summary>
        /// 身分證
        /// </summary>
        public string IdNo { get; set; }

    }
}
