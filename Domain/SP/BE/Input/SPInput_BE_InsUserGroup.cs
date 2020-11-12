using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_InsUserGroup : SPInput_Base
    {

        /// <summary>
        /// 使用者群組代碼
        /// </summary>
        public string UserGroupID { set; get; }
        /// <summary>
        /// 使用者群組名稱
        /// </summary>
        public string UserGroupName { set; get; }
        /// <summary>
        /// 功能群組
        /// </summary>
        public int FuncGroupID { set; get; }
        /// <summary>
        /// 業者別
        /// </summary>
        public int OperatorID { set; get; }
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
