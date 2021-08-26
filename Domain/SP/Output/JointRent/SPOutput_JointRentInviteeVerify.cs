using Domain.SP.Output;
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
    public class SPOutput_JointRentInviteeVerify : SPOutput_Base
    {

        /// <summary>
        /// 被邀請的ID
        /// </summary>
        public string InviteeId { set; get; }
    }
}
