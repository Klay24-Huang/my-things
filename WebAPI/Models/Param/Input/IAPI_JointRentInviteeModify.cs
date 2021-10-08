using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_JointRentInviteeModify
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }
        // <summary>
        /// 被邀請人ID
        /// </summary>
        public string InviteeId { get; set; }

        // <summary>
        /// 行為 (F:取消 D:刪除 S:重新邀請)
        /// </summary>
        public string ActionType { get; set; }
    }
}