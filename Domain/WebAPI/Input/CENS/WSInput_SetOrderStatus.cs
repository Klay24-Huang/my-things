using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 2.2 設定/解除租約
    /// </summary>
    public class WSInput_SetOrderStatus:WSInput_Base
    {
        /// <summary>
        /// 設定/解除租約
        /// <para>0:解除租約</para>
        /// <para>1:設定租約</para>
        /// </summary>
        public int OrderStatus { set; get; }
    }
}
