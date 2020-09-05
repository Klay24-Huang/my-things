using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Common
{
    /// <summary>
    /// 只判斷Token
    /// </summary>
   public class SPInput_CheckTokenOnlyToken
    {
        public string Token { set; get; }
        public Int64 LogID { set; get; }
    }
}
