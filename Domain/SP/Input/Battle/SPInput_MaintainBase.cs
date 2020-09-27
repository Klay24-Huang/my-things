using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Battle
{
    public class SPInput_MaintainBase : SPInput_Base 
    {
        /// <summary>
        /// 驗證金鑰
        /// </summary>
        public string CheckKey { set; get; }
    }
}
