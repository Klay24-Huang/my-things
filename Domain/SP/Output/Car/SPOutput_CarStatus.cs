using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Car
{
    public class SPOutput_CarStatus
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }
        /// <summary>
        /// 引擎狀態，發動為1，熄火為0
        /// </summary>
        public int PowerOnStatus { get; set; }
        /// <summary>
        /// 中控鎖狀態：1為上鎖，0為解鎖
        /// </summary>
        public int CentralLockStatus { get; set; }
        /// <summary>
        /// 車門狀態：1111關門;0000開門
        /// </summary>
        public string DoorStatus { get; set; }
        /// <summary>
        /// 門鎖狀態：1為上鎖，0為解鎖，四個門鎖分別為：駕駛門鎖、副駕駛門、乘客門鎖、後行李箱門鎖
        /// </summary>
        public string LockStatus { get; set; }
        /// <summary>
        /// 車內燈：1為開啟，0為關閉
        /// </summary>
        public int IndoorLightStatus { get; set; }
        /// <summary>
        /// 防盜鎖狀態：1為開啟，0為關閉
        /// </summary>
        public int SecurityStatus { get; set; }

    }
}
