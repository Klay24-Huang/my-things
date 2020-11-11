using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 功能群組
    /// </summary>
    public class BE_GetFuncGroup
    {
        /// <summary>
        /// pk
        /// </summary>
        public int SEQNO { set; get; }
        /// <summary>
        /// 功能群組代碼
        /// </summary>
        public string FuncGroupID { set; get; }
        /// <summary>
        /// 功能群組名稱
        /// </summary>
        public string FuncGroupName { set; get; }
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
