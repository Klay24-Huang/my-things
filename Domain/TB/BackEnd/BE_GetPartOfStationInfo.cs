using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 據點資訊設定列表
    /// </summary>
    public class BE_GetPartOfStationInfo
    {
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string StationID       {set;get;}
        /// <summary>
        /// 據點名稱
        /// </summary>
        public string Location        {set;get;}
        /// <summary>
        /// 地址
        /// </summary>
        public string ADDR            {set;get;}
        /// <summary>
        /// 緯度
        /// </summary>
        public string Latitude        {set;get;}
        /// <summary>
        /// 經度
        /// </summary>
        public string Longitude       {set;get;}
        /// <summary>
        /// 縣市名稱
        /// </summary>
        public string CityName        {set;get;}
        /// <summary>
        /// 行政區名稱
        /// </summary>
        public string AreaName        {set;get;}
        /// <summary>
        /// 車位數
        /// </summary>
        public int AllowParkingNum {set;get;}
        /// <summary>
        /// 目前庫位車輛未上線數
        /// </summary>
        public int UnavailbleCar { set; get; }
        /// <summary>
        /// 當前庫位總車輛數
        /// </summary>
        public int TotalCar { get; set; }

    }
}
