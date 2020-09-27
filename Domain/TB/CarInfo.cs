using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
   public class CarInfo
    {
       
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }
        /// <summary>
        /// 車機類型
        /// <para>汽車：0</para>
        /// <para>機車：1</para>
        /// </summary>
        public int deviceType { get; set; }
        public int ACCStatus { get; set; }
        public int GPSStatus { get; set; }
        public string GPSTime { get; set; }
        public int OBDstatus { get; set; }
        public int GPRSStatus { get; set; }
        public int PowerONStatus { get; set; }
        public int CentralLockStatus { get; set; }
        public string DoorStatus { get; set; }
        public string LockStatus { get; set; }
        public int IndoorLightStatus { get; set; }
        public int SecurityStatus { get; set; }
        public int Speed { get; set; }
        public double Volt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Millage { get; set; }
        public int extDeviceStatus1 { get; set; }
        public int extDeviceStatus2 { get; set; }
        public string extDeviceData2 { get; set; }
        public string extDeviceData3 { get; set; }
        public string extDeviceData4 { get; set; }
        /// <summary>
        /// 讀卡
        /// </summary>
        public string extDeviceData7 { get; set; }
    }
}
