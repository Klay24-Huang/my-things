using System;

namespace Domain.TB.Sync
{
    public class Sync_SendEventMessage
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int64 AlertID { get; set; }

        /// <summary>
        /// 告警類別
        /// <para>1:沒租約但是有時速</para>
        /// <para>6:一般車輛低電量</para>
        /// <para>7:發現車輛無租約但車門被打開</para>
        /// <para>8:發現車輛無租約但電門被啟動</para>
        /// <para>9:發現車輛無租約但引擎被發動</para>
        /// </summary>
        public int EventType { get; set; }

        /// <summary>
        /// 收信者
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 是否發送：0:否;1:是;2:失敗;3:不處理
        /// </summary>
        public int HasSend { get; set; }

        /// <summary>
        /// 寄送時間
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime MKTime { get; set; }
    }

    public class Sync_OrderMain
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderNumber { get; set; }
    }
}