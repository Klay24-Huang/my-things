using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_UPDUserGroup
    {
        /// <summary>
        /// 使用者群組pk
        /// </summary>
        public int SEQNO { set; get; }
        /// <summary>
        /// 使用者群組代碼
        /// </summary>
        public string UserGroupID { set; get; }
        /// <summary>
        /// 使用者群組名稱
        /// </summary>
        public string UserGroupName { set; get; }
        public int FuncGroupID { set; get; }
        /// <summary>
        /// 業者別
        /// </summary>
        public int OperatorID { set; get; }
        /// <summary>
        /// 有效日期（起）
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 有效日期（迄）
        /// </summary>
        public string EndDate { set; get; }
        public string UserID { set; get; }
    }
}