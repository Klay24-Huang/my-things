using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OrderAuth
{
    public class SPInput_OrderNYList_U01 : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string INVNO { set; get; }
    }
}
