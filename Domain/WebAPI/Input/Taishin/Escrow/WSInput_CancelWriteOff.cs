using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Input.Taishin.Escrow
{
    public class WSInput_CancelWriteOff: WSInput_EscrowBase
    {
        /// <summary>
        /// 核銷資料 (禮券序號+核銷碼，傳入張數以100為限)
        /// </summary>
        public string[] WriteOffData { get; set; }
        /// <summary>
        /// 交易來源
        /// </summary>
        public string SourceFrom { get; set; }
    }
}
