using System;

namespace Domain.SP.Input.Booking
{
    public class SPInput_BindUUCardJob : SPInput_Base
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 執行結果(0:未處理 1:成功 2:失敗)
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 悠遊卡卡號
        /// </summary>
        public string CardNo { get; set; }
    }
}