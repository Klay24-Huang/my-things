using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.Output.CENS
{
    /// <summary>
    /// 取得即時狀態
    /// </summary>
    public class WSOutput_GetInfo:WSOutput_Base
    {
        public GetInfoData data { set; get; }
    }
    public class GetInfoData
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { get; set; }
        /// <summary>
        /// OBD狀態（0:發生錯誤;1:正常連線，因直接接收can資訊，所以此欄位代表是否能正常取得CAN資料)
        /// </summary>
        public int OBDStatus { get; set; }
        /// <summary>
        /// GPS狀態（0:發生錯誤;1:正常)
        /// </summary>
        public int GPSStatus { get; set; }
        /// <summary>
        /// GPRS狀態（0:發生錯誤;1:正常)
        /// </summary>
        public int GPRSStatus { get; set; }
        /// <summary>
        /// 電源狀態（0:未發動;1:發動)
        /// </summary>
        public int AccOn { get; set; }
        /// <summary>
        /// 引擎狀態（0:未發動;1:發動)
        /// </summary>
        public int PowOn { get; set; }
        /// <summary>
        /// 目前時速
        /// </summary>
        public double SPEED { get; set; }
        /// <summary>
        /// 目前里程（-1:無法取得)
        /// </summary>
        public double Milage { get; set; }
        /// <summary>
        /// 目前電壓（-1:無法取得)
        /// </summary>
        public double Volt { get; set; }
        /// <summary>
        /// 目前緯度（-1:無法取得)
        /// </summary>
        public decimal Lat { get; set; }
        /// <summary>
        /// 目前經度（-1:無法取得)
        /// </summary>
        public decimal Lng { get; set; }
        /// <summary>
        /// 車門狀態(0000~1111，由左至右分別代表左前<駕駛座>、右前<副駕>、左後、右後)（N:無法取得，0:開門;1:關門)
        /// </summary>
        public string doorStatus { get; set; }
        /// <summary>
        /// 車鎖狀態(0000~1111，由左至右分別代表左前<駕駛座>、右前<副駕>、左後、右後)（N:無法取得，0:解鎖;1:上鎖)
        /// </summary>
        public string lockStatus { get; set; }
        /// <summary>
        /// 租約狀態（0:無租約;1:有租約)
        /// </summary>
        public int OrderStatus { get; set; }
        /// <summary>
        /// 燈狀態（任一燈只要有開就算開啟）（0:關閉;1:開啟)
        /// </summary>
        public int IndoorLight { get; set; }
        /// <summary>
        /// 防盜鎖狀態（0:上鎖;1:未上鎖)
        /// </summary>
        public int SecurityStatus { get; set; }
        /// <summary>
        /// 中控鎖狀態（0:上鎖;1:未上鎖)
        /// </summary>
        public int CentralLock { get; set; }
        /// <summary>
        /// GPS時間
        /// </summary>
        public DateTime GpsTime { get; set; }
        /// <summary>
        /// iButton是否有掛回
        /// <para>0:無</para>
        /// <para>1:有</para>
        /// </summary>
        public int IButton { get; set; }
        /// <summary>
        /// iButtonKey
        /// </summary>
        public string IButtonKey { get; set; }

    }
}
