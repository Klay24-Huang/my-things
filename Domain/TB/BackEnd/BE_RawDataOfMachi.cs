using System;

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

        //20210510唐加
        /// <summary>
        /// 調度停車場
        /// </summary>
        public string PP { set; get; }
        /// <summary>
        /// 場內還車
        /// </summary>
        public string returnFlg { set; get; }
        /// <summary>
        /// 停車場業者
        /// </summary>
        public string OP { set; get; }
    }
}