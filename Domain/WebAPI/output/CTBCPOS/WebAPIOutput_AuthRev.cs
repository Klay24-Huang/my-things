using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.CTBCPOS
{
    /// <summary>
    /// 取消授權回覆結果
    /// </summary>
    public class WebAPIOutput_AuthRev : WebAPIOutput_CTBCBase
    {
        /// <summary>
        /// 原交易銷貨金額
        /// </summary>
        public int PurchAmt { get; set; } = 0;

        /// <summary>
        /// 授權取消的金額
        /// </summary>
        public int AuthNewAmt { get; set; } = 0;

     
    }
}


