using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 案件共同承租人邀請
    /// </summary>
    public class IAPI_JointRentInvitation
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
        /// 要邀請的ID或手機(原input參數)
        /// </summary>
        public string QueryId { set; get; }
    }
}