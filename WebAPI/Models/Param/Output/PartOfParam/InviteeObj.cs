using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    /// <summary>
    /// 共同承租被邀請人
    /// </summary>
    public class InviteeObj
    {
        /// <summary>
        /// 邀請時輸入的ID/手機
        /// </summary>
        public string QueryId { get; set; }
        /// <summary>
        /// 被邀請人ID
        /// </summary>
        public string InviteeId { get; set; }
        /// <summary>
        /// 被邀請人姓名
        /// </summary>
        public string InviteeName { get; set; }
        /// <summary>
        /// 邀請狀態
        /// </summary>
        public string InvitationStatus { get; set; }
    }
}