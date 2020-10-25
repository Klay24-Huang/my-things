using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_Operator
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
        /// 加盟業者icon
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
    }
}
