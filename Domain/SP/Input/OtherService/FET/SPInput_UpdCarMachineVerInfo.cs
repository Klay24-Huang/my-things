using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.FET
{
    /// <summary>
    /// 寫入遠傳車機韌體資訊
    /// </summary>
    public class SPInput_UpdCarMachineVerInfo
    {
        public string deviceName { set; get; }
        public string deviceCID { set; get; }
        public string deviceFW { set; get; }
        public Int64 LogID { set; get; }
    }
}
