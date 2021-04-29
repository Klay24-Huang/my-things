using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台會員審核列表
    /// </summary>
    public class BE_GetEasyWalletList
    {
        /// <summary>
        /// 申請日期
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public string orderNo { set; get; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public string projectName { set; get; }
        /// <summary>
        /// 申請日期
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
    }
}
