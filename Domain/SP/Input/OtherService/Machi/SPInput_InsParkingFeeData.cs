using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.OtherService.Machi
{
    public class SPInput_InsParkingFeeData:SPInput_Base
    {
        /// <summary>
        /// 車麻吉訂單編號
        /// </summary>
        public string machi_id { set; get; }
        /// <summary>
        /// 車麻吉停車場代碼
        /// </summary>
	    public string machi_parking_id { set; get; }
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 停車費
        /// </summary>
        public int Amount { set; get; }
        /// <summary>
        /// 入場時間
        /// </summary>
        public DateTime Check_in { set; get; }
        /// <summary>
        /// 出場時間
        /// </summary>
        public DateTime Check_out { set; get; }
        
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }
    }
}
