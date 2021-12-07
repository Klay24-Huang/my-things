using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.CTBCPOS
{
    /// <summary>
    /// 取消授權
    /// </summary>
    public class WebAPIInput_AuthRev : WebAPIInput_CTBCBase
    {
      
        /// <summary>
        /// 更正的授權金額
        /// </summary>
        public int AuthNewAmt { get; set; } = 0;

    }
}
