using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 案件共同承租人回應邀請
    /// </summary>
    public class IAPI_JointRentIviteeFeedBack
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 被邀請的ID
        /// </summary>
        public string InviteeId { set; get; }

        /// <summary>
        /// 邀請回覆 (Y:同意  N:拒絕)
        /// </summary>
        public string FeedbackType { set; get; }
    }
}