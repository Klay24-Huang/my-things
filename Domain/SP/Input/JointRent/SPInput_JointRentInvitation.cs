using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.JointRent
{
    /// <summary>
    /// 案件共同承租人邀請
    /// </summary>
    public class SPInput_JointRentInvitation : SPInput_Base
    {

        /// <summary>
        /// 身分證/居留證號
        /// </summary>
        public string IDNO { set; get; }

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
        /// 要邀請的ID或手機(原input參數)
        /// </summary>
        public string QueryId { set; get; }
    }
}
