using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Member
{
    public class SPInput_SetDefPayMode:SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 預設的支付方式
        /// <para>0:信用卡</para>
        /// <para>1:和雲錢包</para>
        /// <para>2:line pay</para>
        /// <para>3:街口支付</para>
        /// </summary>
        public int PayMode { set; get; }
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }
    }
}
