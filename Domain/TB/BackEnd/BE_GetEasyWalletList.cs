using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 悠遊付退款查詢
    /// </summary>
    public class BE_GetEasyWalletList
    {
        /// <summary>
        /// orderNo
        /// </summary>
        public string orderNo { set; get; }
        /// <summary>
        /// 方案名稱
        /// </summary>
        public string projectName { set; get; }
        /// <summary>
        /// IDNO
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 購買日
        /// </summary>
        public string orderTime { set; get; }
        /// <summary>
        /// merchantOrderNo
        /// </summary>
        public string merchantOrderNo { set; get; }
        /// <summary>
        /// 客人名字
        /// </summary>
        public string MEMCNAME { set; get; }
        /// <summary>
        /// 錢錢
        /// </summary>
        public string orderAmount { set; get; }
        public string endTime { set; get; }
    }
}
