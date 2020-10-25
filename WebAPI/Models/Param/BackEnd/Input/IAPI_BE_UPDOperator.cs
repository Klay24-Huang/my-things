using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    /// <summary>
    /// 更新加盟業者資訊
    /// </summary>
    public class IAPI_BE_UPDOperator:IAPI_BE_Base
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
        public string OldOperatorIcon { set; get; }
        /// <summary>
        /// 有效日期（起）
        /// </summary>
        public string StartDate { set; get; }
        /// <summary>
        /// 有效日期（迄）
        /// </summary>
        public string EndDate { set; get; }


    }
}