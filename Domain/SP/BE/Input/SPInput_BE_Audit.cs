using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_Audit:SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 駕照類型
        /// </summary>
        public string Driver { get; set; }
        /// <summary>
        /// 特殊身份
        /// </summary>
        public string SPECSTATUS { get; set; }
        /// <summary>
        /// 特殊身份起日
        /// </summary>
        public string SPSD { get; set; }
        /// <summary>
        /// 特殊身份迄日
        /// </summary>
        public string SPED { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string Birth { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 區域代碼
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr { get; set; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UniCode { get; set; }
        /// <summary>
        /// 發票類型
        /// </summary>
        public string InvoiceType { get; set; }
        /// <summary>
        /// 審核狀態
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>
        /// 審核不通過原因
        /// </summary>
        public string NotAuditReason { get; set; }
        /// <summary>
        /// 審核不通過原因說明
        /// </summary>
        public string RejectReason { get; set; }
        /// <summary>
        /// 是否為新註冊
        /// </summary>
        public int IsNew { set; get; }
        public string UserID { set; get; }

        //20201125 UPD BY JERRY 增加欄位處理
        public string MEMHTEL { get; set; }
        public string MEMCOMTEL { get; set; }
        public string MEMCONTRACT { get; set; }
        public string MEMCONTEL { get; set; }
        public string MEMEMAIL { get; set; }
        public int HasVaildEMail { get; set; }
        public string MEMMSG { get; set; }
        //20201125 UPD BY 堂尾鰭 增加欄位處理
        public string MEMONEW { get; set; }
    }
}
