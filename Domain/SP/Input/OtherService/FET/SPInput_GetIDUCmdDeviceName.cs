using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.FET
{
    public class SPInput_GetIDUCmdDeviceName
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }
        public Int64 LogID { set; get; }
    }
}
