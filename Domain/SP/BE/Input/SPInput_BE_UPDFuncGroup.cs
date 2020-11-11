using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_UPDFuncGroup:SPInput_Base
    {
        /// <summary>
        /// 功能群組pk
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
        public string UserID { set; get; }
    }
}
