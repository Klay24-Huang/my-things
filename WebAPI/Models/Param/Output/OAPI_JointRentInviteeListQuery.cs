using System.Collections.Generic;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_JointRentInviteeListQuery
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 共同承租人邀請清單
        /// </summary>
        public List<InviteeObj> Invitees { get; set; }
    }
}