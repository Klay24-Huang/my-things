using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CarMachine
{
    public class BLEInfo
    {
        /// <summary>
        /// 藍芽帳號
        /// </summary>
        public string BLE_Device { set; get; }
        /// <summary>
        /// 藍芽密碼
        /// </summary>
        public string BLE_PWD { set; get; }
    }
}
