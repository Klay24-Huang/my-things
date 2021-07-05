using System;

namespace Domain.SP.Output.Member
{
    public class MemberScoreList
    {
        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 編號
        /// </summary>
        public int RowNo { get; set; }

        /// <summary>
        /// 取得日期
        /// </summary>
        public DateTime GetDate { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int SEQ { get; set; }

        /// <summary>
        /// 分數
        /// </summary>
        public int SCORE { get; set; }

        /// <summary>
        /// 用戶畫面敘述
        /// </summary>
        public string UIDESC { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string ORDERNO { get; set; }
    }
}
