using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Mochi
{
    /// <summary>
    /// 車麻吉取得token
    /// </summary>
    public class SPInput_GetMachiToken:SPInput_Base
    {
        public string NowTime { set; get; }
    }
}
