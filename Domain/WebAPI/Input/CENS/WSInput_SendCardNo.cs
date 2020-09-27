using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CENS
{
    /// <summary>
    /// 2.3設定/解除卡號
    /// </summary>
    public class WSInput_SendCardNo:WSInput_Base
    {
        /// <summary>
        /// 模式
        /// <para>0:解除卡號</para>
        /// <para>1:設定卡號</para>
        /// </summary>
        public int mode { set; get; }

        public SendCarNoData[] data { set; get; } 
    }
    public class SendCarNoData
    {
        /// <summary>
        /// 卡號類型
        /// <para>0:萬用卡</para>
        /// <para>1:顧客卡</para>
        /// </summary>
        public int CardType { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
    }
}
