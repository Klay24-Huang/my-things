using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_RawDataOfMachi
    {
        /// <summary>
        /// 車麻吉訂單編號
        /// </summary>
        public string machi_id { set; get; }
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 停車場名稱
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 入場時間
        /// </summary>
        public DateTime Check_in { set; get; }
        /// <summary>
        /// 出場時間
        /// </summary>
        public DateTime Check_out { set; get; }
        /// <summary>
        /// iRent取車時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// iRent還車時間
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 停車場費用
        /// </summary>
        public string Amount { set; get; }
        /// <summary>
        /// 優惠折扣
        /// </summary>
        public int refund_amount { set; get; }
        /// <summary>
        /// 付款時間
        /// </summary>
        public DateTime paid_at { set; get; }
        /// <summary>
        /// 違規
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int Conviction { set; get; }
    }
}
