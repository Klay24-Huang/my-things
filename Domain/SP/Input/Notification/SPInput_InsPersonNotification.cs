using System;

namespace Domain.SP.Input.Notification
{
    public class SPInput_InsPersonNotification : SPInput_Base
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 類別
        /// <para>0 公告</para>
        /// <para>1 取車通知</para>
        /// <para>2 還車通知</para>
        /// <para>3 逾期未取車</para>
        /// <para>4 逾時通知</para>
        /// <para>5 結帳15分鐘未還車</para>
        /// <para>9 前預約未還車</para>
        /// <para>15 電源未關</para>
        /// <para>16 大燈未關</para>
        /// <para>18 好友推薦</para>
        /// </summary>
        public int NType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime STime { get; set; }

        /// <summary>
        /// 主旨
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 內文
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// IMAGE URL
        /// </summary>
        public string imageurl { get; set; }
    }
}