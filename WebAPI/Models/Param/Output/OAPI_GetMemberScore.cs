using Domain.SP.Output.Member;
using System.Collections.Generic;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetMemberScore
    {
        /// <summary>
        /// 會員積分
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { set; get; }

        /// <summary>
        /// 明細列表
        /// </summary>
        public List<MemberScoreList> DetailList { get; set; }
    }
}