using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 車輛事件狀態
    /// </summary>
    public class BE_CarEventLog
    {
        /// <summary>
        /// 
        /// </summary>
        public int deviceType { set; get; }
        public string CarNo { set; get; }
        public string CID { set; get; }
        /// <summary>
        /// 車輛發動狀態
        /// <para>0:熄火</para>
        /// <para>1:發動</para>
        /// </summary>
        public int ACCStatus { set; get; }
        /// <summary>
        /// GPS狀態
        /// <para>0:無效</para>
        /// <para>1:有效</para>
        /// </summary>
        public int GPSStatus { set; get; }
        /// <summary>
        /// OBD狀態(汽車專屬)，休眠時也會為0
        /// <para>0:無效</para>
        /// <para>1:有效</para>
        /// </summary>
        public int OBDStatus { set; get; }
        /// <summary>
        /// GPRS狀態(汽車專屬)
        /// <para>0:離線</para>
        /// <para>1:上線</para>
        /// </summary>
        public int GPRSStatus { set; get; }
        /// <summary>
        /// 車輛引擎狀態(汽車專屬)
        /// <para>0:熄火</para>
        /// <para>1:發動</para>
        /// </summary>
        public int PowerOnStatus { set; get; }
        /// <summary>
        /// 中控鎖狀態(汽車專屬)
        /// <para>0:解鎖</para>
        /// <para>1:上鎖</para>
        /// </summary>
        public int CentralLockStatus { set; get; }
        /// <summary>
        /// 車內燈狀態(汽車專屬)
        /// <para>0:關閉</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int IndoorLightStatus { set; get; }
        /// <summary>
        /// 車門狀態
        /// <para>0:關</para>
        /// <para>1:開</para>
        /// </summary>
        public int DoorStatus { set; get; }
        /// <summary>
        /// 防盜鎖狀態(汽車專屬)
        /// <para>0:關閉</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int SecurityStatus { set; get; }
        /// <summary>
        /// 機車專用，電池架
        /// <para>0:未開啟</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int BatCover { set; get; }
        /// <summary>
        /// 機車專用，低電壓警示
        /// <para>0:未符合低電壓</para>
        /// <para>1:低電壓</para>
        /// </summary>
        public int MotorLowVol { set; get; }
        /// <summary>
        /// 寫入時間
        /// </summary>
        public DateTime MKTime { set; get; }
    }
}
