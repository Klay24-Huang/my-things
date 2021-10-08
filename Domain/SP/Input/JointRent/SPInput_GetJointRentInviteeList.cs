using System;


namespace Domain.SP.Input.JointRent
{
    public class SPInput_GetJointRentInviteeList : SPInput_Base
    {
        /// <summary>
        /// JWT TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 操作的會員帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { get; set; }
    }
}
