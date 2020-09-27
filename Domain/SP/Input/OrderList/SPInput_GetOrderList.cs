using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OrderList
{
    public class SPInput_GetOrderList:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
    }
}
