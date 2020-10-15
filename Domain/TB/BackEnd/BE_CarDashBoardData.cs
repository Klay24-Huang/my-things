using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_CarDashBoardData
    {
        /// <summary>
        /// 目前所在據點代碼
        /// </summary>
        public string StationID { set; get; }
        /// <summary>
        /// 目前所在據點名稱
        /// </summary>
        public string StationName { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 目前狀態
        /// <para>0:出租中</para>
        /// <para>1:可出租</para>
        /// <para>2:待上線</para>
        /// </summary>
        public Int16 NowStatus { set; get; }
        /// <summary>
        /// 車輛類型
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// </summary>
        public int deviceType { set; get; }
        /// <summary>
        /// 遠傳車機token，興聯的沒有
        /// </summary>
        public string deviceToken { set; get; }
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
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
        /// 車速
        /// </summary>
        public int Speed { set; get; }
        /// <summary>
        /// 電壓
        /// </summary>
        public float Volt { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 里程數
        /// </summary>
        public float Millage { set; get; }
        /// <summary>
        /// 二顆電池平均(機車專用)
        /// </summary>
        public float TBA2 { set; get; }
        /// <summary>
        /// 三顆電池平均(機車專用)
        /// </summary>
        public float TBA3 { set; get; }
        /// <summary>
        /// 核心電池(機車專用)
        /// </summary>
        public float MBA { set; get; }
        /// <summary>
        /// 右側電池(機車專用)
        /// </summary>
        public float RBA { set; get; }
        /// <summary>
        /// 左側電池(機車專用)
        /// </summary>
        public float LBA { set; get; }
        /// <summary>
        /// 機車專用，使用者連接藍牙模組
        /// <para>0:未連接</para>
        /// <para>1:連接</para>
        /// </summary>
        public int BLELogin { set; get; }
        /// <summary>
        /// 機車專用，藍牙廣播
        /// <para>0:未開啟</para>
        /// <para>1:開啟</para>
        /// </summary>
        public int BroadCast { set; get; }
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
        /// 車機是否是租約狀態
        /// </summary>
        public int CarRent { set; get; }
        /// <summary>
        /// 汽車專用，iButton是否扣回
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int iButton { set; get; }
        /// <summary>
        /// 汽車專用，iButton編號
        /// </summary>
        public string iButtonKey { set; get; }
        /// <summary>
        /// 汽車專用，目前車機寫入的卡號
        /// </summary>
        public string CustomerCardNo { set; get; }
        /// <summary>
        /// 機車專用，ble device name
        /// </summary>
        public string BLE_DeviceName { set; get; }
        /// <summary>
        /// 機車專用，此租約的ble密碼
        /// </summary>
        public string BLE_DeviceKey { set; get; }
        /// <summary>
        /// GPS時間(null以空字串)
        /// </summary>
        public string GPSTime { set; get; }
        /// <summary>
        /// 最近一次收到更新時間(null以空字串)
        /// </summary>
        public string LastUpdate { set; get; }
        /// <summary>
        /// 是否為機車
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 isMoto { set; get; }
        /// <summary>
        /// 是否為興聯車機
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsCens { set; get; }
    }
}
