using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsTransParking_Moto : SPInput_Base
    {

        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string ParkingName { set; get; }
        /// <summary>
        /// 機車專用格數
        /// </summary>
        public string ParkingCNT { set; get; }
        /// <summary>
        /// 停車場地址
        /// </summary>
        public string ParkingAddress { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal ParkingLng { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal ParkingLat { set; get; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime OpenTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime CloseTime { set; get; }
        /// <summary>
        /// 操作者帳號
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 備註
        /// </summary>
        public string ParkingMark { set; get; }
        /// <summary>
        /// ID
        /// </summary>
        public string ParkingID { set; get; }
    }
}
