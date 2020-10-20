using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 萬用卡資料（未分類）
    /// </summary>
    public class BE_MasterCarDataOfPart
    {
        /// <summary>
        /// 員工帳號
        /// </summary>
        public string ManagerId { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        ///// <summary>
        ///// 車機編號
        ///// </summary>
        //public string CID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
    }
}
