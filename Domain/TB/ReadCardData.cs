using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB
{
    /// <summary>
    /// 取得讀卡資料
    /// </summary>
    public class ReadCardData
    {
        /// <summary>
        /// 車機編號
        /// </summary>
        public string CID { set; get; }
        /// <summary>
        /// 讀到的卡號
        /// </summary>
        public string CardNo { set; get; }
    }
}
