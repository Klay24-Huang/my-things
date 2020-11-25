using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
   public class MotorInfo
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
        /// GPRS狀態：上線為1，離線為0
        /// </summary>
        public int GPRSStatus { get; set; }
        /// <summary>
        /// GPS定位時間，utc
        /// </summary>
        public string GPSTime { get; set; }
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
        public double Millage { get; set; }
        /// <summary>
        /// 方向角
        /// </summary>
        public double deviceCourse { get; set; }
        /// <summary>
        /// 轉速
        /// </summary>
        public int deviceRPM { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int deviceiSpeed { get; set; }
        /// <summary>
        /// 總電量兩顆平均
        /// </summary>
        public double device2TBA { get; set; }
        /// <summary>
        /// 總電量三顆平均
        /// </summary>
        public double device3TBA { get; set; }
        /// <summary>
        /// 總電量
        /// </summary>
        public string deviceRSOC { get; set; }
        /// <summary>
        /// 預估里程
        /// </summary>
        public string deviceRDistance { get; set; }
        /// <summary>
        /// 核心電池
        /// </summary>
        public double deviceMBA { get; set; }
        /// <summary>
        /// 核心電池電流
        /// </summary>
        public int deviceMBAA { get; set; }
        /// <summary>
        /// 核心電池最高溫度
        /// </summary>
        public int deviceMBAT_Hi { get; set; }
        /// <summary>
        /// 核心電池最低溫度
        /// </summary>
        public int deviceMBAT_Lo { get; set; }
        /// <summary>
        /// 右邊電池
        /// </summary>
        public double deviceRBA { get; set; }
        /// <summary>
        /// 右共享電池電流
        /// </summary>
        public int deviceRBAA { get; set; }
        /// <summary>
        /// 右共享電池最高溫度
        /// </summary>
        public int deviceRBAT_Hi { get; set; }
        /// <summary>
        /// 右共享電池最低溫度
        /// </summary>
        public int deviceRBAT_Lo { get; set; }
        /// <summary>
        /// 左共享電池
        /// </summary>
        public double deviceLBA { get; set; }
        /// <summary>
        /// 左共享電池電流
        /// </summary>
        public int deviceLBAA { get; set; }
        /// <summary>
        /// 左共享電池最高溫度
        /// </summary>
        public int deviceLBAT_Hi { get; set; }
        /// <summary>
        /// 左共享電池最低溫度
        /// </summary>
        public int deviceLBAT_Lo { get; set; }
        /// <summary>
        /// 直流測大電容溫度
        /// </summary>
        public int deviceTMP { get; set; }
        /// <summary>
        /// 馬達端電流
        /// </summary>
        public int deviceCur { get; set; }
        /// <summary>
        /// 把手位置
        /// </summary>
        public int deviceTPS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int deviceiVOL { get; set; }
        /// <summary>
        /// 現狀故障碼
        /// </summary>
        public int deviceErr { get; set; }
        /// <summary>
        /// 現狀警告碼
        /// </summary>
        public int deviceALT { get; set; }
        /// <summary>
        /// 三軸平方和最大值的X軸G 值
        /// </summary>
        public double deviceGx { get; set; }
        /// <summary>
        /// 三軸平方和最大值的Y軸G 值
        /// </summary>
        public double deviceGy { get; set; }
        /// <summary>
        /// 三軸平方和最大值的Z軸G 值
        /// </summary>
        public double deviceGz { get; set; }
        /// <summary>
        /// 使用者連接藍牙模組：1:有;0:否
        /// </summary>
        public int deviceBLE_Login { get; set; }
        /// <summary>
        /// 藍牙廣播：1:有;0:否
        /// </summary>
        public int deviceBLE_BroadCast { get; set; }
        /// <summary>
        /// Power Mode，1:有;0:無
        /// </summary>
        public int devicePwr_Mode { get; set; }
        /// <summary>
        /// 倒車檔狀態
        /// </summary>
        public int deviceReversing { get; set; }
        /// <summary>
        /// 腳架狀態
        /// </summary>
        public int devicePut_Down { get; set; }
        /// <summary>
        /// Power Relay
        /// </summary>
        public int devicePwr_Relay { get; set; }
        /// <summary>
        /// 馬達
        /// </summary>
        public int deviceStart_OK { get; set; }
        /// <summary>
        /// 急加速
        /// </summary>
        public int deviceHard_ACC { get; set; }
        /// <summary>
        /// 急煞
        /// </summary>
        public int deviceEMG_Break { get; set; }
        /// <summary>
        /// 急轉彎
        /// </summary>
        public int deviceSharp_Turn { get; set; }
        /// <summary>
        /// 電池蓋
        /// </summary>
        public int deviceBat_Cover { get; set; }
        /// <summary>
        /// 低電壓警示（機車專用）
        /// </summary>
        public int deviceLowVoltage { set; get; }
        /// <summary>
        /// 租約狀態
        /// </summary>
        public int extDeviceStatus1 { get; set; }
        /// <summary>
        /// 汽車專用，iButton扣壓，是為1，否為0
        /// </summary>
        public string extDeviceData2 { get; set; }
        /// <summary>
        /// 機車專用，租約成立時，回傳藍芽廣播名稱
        /// </summary>
        public string extDeviceData5 { set; get; }
        /// <summary>
        /// 機車專用，租約成立時，回傳藍芽廣播密碼
        /// </summary>
        public string extDeviceData6 { set; get; }
    }
}