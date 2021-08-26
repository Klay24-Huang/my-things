using Domain.TB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 共同承租人邀請檢核輸出
    /// </summary>
    public class OAPI_JointRentInviteeVerify
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }

        /// <summary>
        /// 被邀請人ID
        /// </summary>
        public string InviteeId { set; get; }

        /// <summary>
        /// 要邀請的ID或手機(原input參數)
        /// </summary>
        public string QureyId { set; get; }

    }

}