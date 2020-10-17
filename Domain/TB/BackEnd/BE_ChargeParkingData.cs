using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public  class BE_ChargeParkingData
    {
        /// <summary>
        /// tb pk
        /// </summary>
        public int ParkId { set; get; }
        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string ParkingName { set; get; }
        /// <summary>
        /// 營運商
        /// </summary>
        public string Operator { set; get; }
        /// <summary>
        /// 經度
        /// </summary>
        public decimal Longitude { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public decimal Latitude { set; get; }
       
        /// <summary>
        /// 停車場地址
        /// </summary>
        public string ParkingAddress { set; get; }
        /// <summary>
        /// 收費金額
        /// </summary>
        public int SettingPrice { set; get; }
        #region 暫時封印
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { set; get; }
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime CloseTime { set; get; }
        #endregion
    }
}
