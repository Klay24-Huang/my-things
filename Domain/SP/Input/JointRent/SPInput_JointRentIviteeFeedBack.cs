using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.JointRent
{
    /// <summary>
    /// 案件共同承租人回應邀請
    /// </summary>
    public class SPInput_JointRentIviteeFeedBack : SPInput_Base
    {

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { set; get; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }

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
