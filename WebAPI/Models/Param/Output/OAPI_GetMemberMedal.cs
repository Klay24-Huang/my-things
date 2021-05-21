using Domain.SP.Output.Member;
using System.Collections.Generic;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMemberMedal
    {
        /// <summary>
        /// 徽章清單
        /// </summary>
        public List<MemberMedalList> MedalList { get; set; }
    }
}