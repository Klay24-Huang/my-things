using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    /// <summary>
    /// 共用代碼
    /// </summary>
    public class CodeData
    {
        /// <summary>
        /// 功能值(流水號)
        /// </summary>
        public Int64 CodeId { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string CodeNm { get; set; }
        /// <summary>
        /// 功能群組
        /// </summary>
        public string CodeGroup { get; set; }
        /// <summary>
        /// 對應Table欄位值
        /// </summary>
        public string MapCode { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string CodeDisc { get; set; }
        /// <summary>
        /// 對應Table
        /// </summary>
        public string TBMap { get; set; }
        /// <summary>
        /// 對應Table欄位
        /// </summary>
        public string TBFieldMap { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}

