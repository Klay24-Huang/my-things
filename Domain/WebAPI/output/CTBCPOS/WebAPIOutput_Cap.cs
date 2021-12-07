using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.CTBCPOS
{
    /// <summary>
    /// 請款回覆結果
    /// </summary>
    public class WebAPIOutput_Cap : WebAPIOutput_CTBCBase
    {
     

        /// <summary>
        /// 授權金額
        /// </summary>
        public int PurchAmt { get; set; } = 0;

        /// <summary>
        /// 回覆請款金額
        /// </summary>
        public int CapAmt { get; set; } = 0;
   

    }
}


