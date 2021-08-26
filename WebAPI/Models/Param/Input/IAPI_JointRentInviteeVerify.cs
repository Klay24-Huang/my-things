using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 共同承租人邀請檢核
    /// </summary>
    public class IAPI_JointRentInviteeVerify
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 要邀請的ID或手機
        /// </summary>
        public string QureyId { set; get; }
    }
}