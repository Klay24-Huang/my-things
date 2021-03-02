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

        /// <summary>
        /// 車輛發動狀態，發動為1，熄火為0
        /// </summary>
        public int ACCStatus { get; set; }

        /// <summary>
        /// GPS狀態：有效為1，無效為0
        /// </summary>
        public int GPSStatus { get; set; }

        /// <summary>
        /// GPS定位時間，utc
        /// </summary>
        public string GPSTime { get; set; }

        /// <summary>
        /// OBD狀態，1上線;0:離線
        /// </summary>
        public int OBDstatus { get; set; }

        /// <summary>
        /// GPRS狀態：上線為1，離線為0
        /// </summary>
        public int GPRSStatus { get; set; }

        /// <summary>
        /// 引擎狀態，發動為1，熄火為0
        /// </summary>
        public int PowerONStatus { get; set; }

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

        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 電壓
        /// </summary>
        public double Volt { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public int Millage { get; set; }

        /// <summary>
        /// 租約狀態
        /// </summary>
        public int extDeviceStatus1 { get; set; }

        /// <summary>
        /// 汽車專用，iButton扣壓，是為1，否為0
        /// </summary>
        public int extDeviceStatus2 { get; set; }

        /// <summary>
        /// GCP ID
        /// </summary>
        public string extDeviceData2 { get; set; }

        /// <summary>
        /// 汽車專用，iButton編號
        /// </summary>
        public string extDeviceData3 { get; set; }

        /// <summary>
        /// 汽車專用，顧客卡號，若有多個卡號，請用英文逗點分隔 (e.g. “1234567890,2468013579”)
        /// </summary>
        public string extDeviceData4 { get; set; }

        /// <summary>
        /// 讀卡
        /// </summary>
        public string extDeviceData7 { get; set; }
    }
}