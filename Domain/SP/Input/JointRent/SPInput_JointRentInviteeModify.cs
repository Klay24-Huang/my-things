using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.JointRent
{
    public class SPInput_JointRentInviteeModify : SPInput_Base
    {
       
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }

        /// <summary>
        /// 邀請的會員帳號
        /// </summary>
        public string InviteeId { get; set; }

        /// <summary>
        /// 行為  行為 (F:取消 D:刪除 S:重新邀請)
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { get; set; }

    }
}
