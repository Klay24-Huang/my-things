using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Common
{
    /// <summary>
    /// TOKEN檢核
    /// </summary>
    public class SPInput_CheckTokenDevice
    {
        public string Token { set; get; }
        public string DeviceID { set; get; }
        public Int64 LogID { set; get; }
    }
}
