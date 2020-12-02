using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Bill
{
    public class SPInput_SetRewardResult
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public long OrderNo { set; get; }
        /// <summary>
        /// 發送結果
        /// </summary>
        public int Result { set; get; }
        public Int64 LogID { set; get; }
    }
}
