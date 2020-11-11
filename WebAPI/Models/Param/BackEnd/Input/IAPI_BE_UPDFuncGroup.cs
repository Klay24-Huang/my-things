using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_UPDFuncGroup : IAPI_BE_Base
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
        public string StartDate { set; get; }
        /// <summary>
        /// 有效日期（迄）
        /// </summary>
        public string EndDate { set; get; }

    }
}