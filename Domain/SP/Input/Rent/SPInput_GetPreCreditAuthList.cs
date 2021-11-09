using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
     public class SPInput_GetPreCreditAuthList: SPInput_Base
    {
        /// <summary>
        /// 取得取車前n小時預約的訂單
        /// </summary>
        public int NHour { get; set; }
    }
}
