using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.MonthlyRent
{
    public class SPInput_GetMonthlyDetail : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64? OrderNo { get; set; }

        /// <summary>
        /// 會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 時數使用開始日
        /// </summary>
        public DateTime? SD { get; set; }

        /// <summary>
        /// 時數使用結束時間
        /// </summary>
        public DateTime? ED { get; set; }

    }
}
