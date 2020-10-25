using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    /// <summary>
    /// 更新加盟業者資訊
    /// </summary>
   public  class SPInput_BE_UPDOperator:SPInput_Base
    {
        /// <summary>
        /// 加盟業者id
        /// </summary>
        public Int64 OperatorID { set; get; }
        /// <summary>
        /// 加盟業者編號
        /// </summary>
        public string OperatorAccount { set; get; }
        /// <summary>
        /// 加盟業者名稱
        /// </summary>
        public string OperatorName { set; get; }
        /// <summary>
        /// 加盟業者icon，如果是空的不修改
        /// </summary>
        public string OperatorICon { set; get; }
        /// <summary>
        /// 有效日期（起）
        /// </summary>
        public DateTime StartDate { set; get; }
        /// <summary>
        /// 有效日期（迄）
        /// </summary>
        public DateTime EndDate { set; get; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string UserID { set; get; }
    }
}
